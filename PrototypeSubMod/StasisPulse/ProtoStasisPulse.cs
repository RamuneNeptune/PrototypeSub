using PrototypeSubMod.Upgrades;
using System.Collections;
using System.Collections.Generic;
using PrototypeSubMod.PowerSystem;
using UnityEngine;

namespace PrototypeSubMod.StasisPulse;

internal class ProtoStasisPulse : ProtoUpgrade
{
    private const int FREEZE_STEPS = 8;
    
    [SerializeField] private AnimationCurve sphereRadius;
    [SerializeField] private Gradient colorOverLifetime;
    [SerializeField] private PowerRelay powerRelay;
    [SerializeField] private FMOD_CustomEmitter activationSfx;
    [SerializeField] private VoiceNotification activationVoiceline;
    [SerializeField] private float activationDelay;
    [SerializeField] private int chargeConsumptionAmount;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float sphereGrowTime;
    [SerializeField] private float minFreezeTime;
    [SerializeField] private float maxFreezeTime;
    [SerializeField] private Renderer sphereVisual;

    private float CurrentDiameter => sphereRadius.Evaluate(currentSphereGrowTimeTime / sphereGrowTime);

    private List<FlashingLightHelpers.ShaderVector4ScalerToken> textureSpeedTokens;
    private GameObject freezeFX;
    private GameObject unfreezeFX;
    private SubRoot subRoot;
    
    private float currentCooldownTime;
    private float currentSphereGrowTimeTime;
    private bool deployingLastFrame;
    private bool activating;
    private Collider[] latestColliders;
    private LayerMask freezeMask;

    private void Start()
    {
        latestColliders = new Collider[1500];
        freezeMask = int.MaxValue;
        freezeMask &= ~(1 << LayerID.Vehicle); // Remove Vehicle layer from mask
        freezeMask &= ~(1 << LayerID.TerrainCollider); // Remove Terrain layer from mask
        freezeMask &= ~(1 << LayerID.Player);
        freezeMask &= ~(1 << LayerID.UI);
        freezeMask &= ~(1 << LayerID.OnlyVehicle);
    }
    
    private void OnEnable()
    {
        UWE.CoroutineHost.StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        subRoot = GetComponentInParent<SubRoot>();
        
        if (freezeFX) yield break;
        
        var rifleTask = CraftData.GetPrefabForTechTypeAsync(TechType.StasisRifle);
        yield return rifleTask;

        var stasisRifle = rifleTask.GetResult();
        var stasisSphere = stasisRifle.GetComponent<StasisRifle>().effectSpherePrefab.GetComponent<StasisSphere>();

        freezeFX = stasisSphere.vfxFreeze;
        unfreezeFX = stasisSphere.vfxUnfreeze;

        var stasisMaterials = stasisSphere.GetComponent<Renderer>().materials;
        Material[] newMaterials = new Material[stasisMaterials.Length];

        for (int i = 0; i < stasisMaterials.Length; i++)
        {
            newMaterials[i] = Instantiate(stasisMaterials[i]);
        }

        sphereVisual.materials = newMaterials;
        sphereVisual.GetComponent<MeshFilter>().mesh = stasisSphere.GetComponent<MeshFilter>().mesh;
        textureSpeedTokens = FlashingLightHelpers.CreateUberShaderVector4ScalerTokens(new Material[]
        {
            sphereVisual.materials[0],
            sphereVisual.materials[1]
        });

        MiscSettings.isFlashesEnabled.changedEvent.AddHandler(this, OnFlashesEnabledChanged);
        UpdateTextureSpeed();

        currentSphereGrowTimeTime = sphereGrowTime;
        sphereVisual.enabled = false;
    }

    private void LateUpdate()
    {
        sphereVisual.enabled = currentSphereGrowTimeTime < sphereGrowTime;
        UpdateMaterials();

        if (!upgradeInstalled)
        {
            return;
        }

        if (currentCooldownTime > 0)
        {
            currentCooldownTime -= Time.deltaTime;
            return;
        }

        HandleSphereSize();
    }

    private void UpdateMaterials()
    {
        if (sphereVisual.materials.Length != 2) return;

        Color color = colorOverLifetime.Evaluate(currentSphereGrowTimeTime / sphereGrowTime);
        sphereVisual.materials[0].SetColor(ShaderPropertyID._Color, color);
        sphereVisual.materials[1].SetColor(ShaderPropertyID._Color, color);
    }

    private void HandleSphereSize()
    {
        if (currentSphereGrowTimeTime < sphereGrowTime)
        {
            currentSphereGrowTimeTime += Time.deltaTime;
            sphereVisual.transform.localScale = Vector3.one * CurrentDiameter;
            deployingLastFrame = true;
        }
        else if (deployingLastFrame)
        {
            currentCooldownTime = cooldownTime;
            deployingLastFrame = false;
        }
    }

    private IEnumerator StartFreezeChecks()
    {
        const int freezeCount = 4;
        for (int i = 0; i < freezeCount; i++)
        {
            yield return new WaitForSeconds(sphereGrowTime / freezeCount);
            HandleFreezing();
        }
    }
 
    private void HandleFreezing()
    {
        int colliderCount = Physics.OverlapSphereNonAlloc(sphereVisual.transform.position, CurrentDiameter / 2f, latestColliders, freezeMask);
        UWE.CoroutineHost.StartCoroutine(FreezeObjectsAsync(colliderCount));
    }

    private IEnumerator FreezeObjectsAsync(int colliderCount)
    {
        int increment = colliderCount / FREEZE_STEPS;
        int lastIncrement = colliderCount - increment * (FREEZE_STEPS - 1);
        for (int i = 0; i < FREEZE_STEPS; i++)
        {
            int currentIncrement = i == FREEZE_STEPS - 1 ? lastIncrement : increment;
            
            for (int j = 0; j < currentIncrement; j++)
            {
                TryFreeze(latestColliders[j]);
            }

            yield return new WaitForEndOfFrame();
        }
    }
    
    private void TryFreeze(Collider collider)
    {
        Rigidbody rigidbody = collider.GetComponentInParent<Rigidbody>();
        if (!rigidbody) return;

        if (rigidbody.isKinematic) return;
        
        if (rigidbody.GetComponentInChildren<ProtoStasisPulse>() != null) return;

        if (rigidbody.TryGetComponent<ProtoStasisFreeze>(out _)) return;

        if (collider == Player.mainCollider) return;
        
        var freeze = rigidbody.gameObject.AddComponent<ProtoStasisFreeze>();
        freeze.SetFreezeTimes(minFreezeTime, maxFreezeTime);
        freeze.SetUnfreezeVF(unfreezeFX);

        Utils.PlayOneShotPS(freezeFX, rigidbody.transform.position, Quaternion.identity);
    }

    private void OnFlashesEnabledChanged(Utils.MonitoredValue<bool> isFlashesEnabled)
    {
        UpdateTextureSpeed();
    }

    private void UpdateTextureSpeed()
    {
        if (MiscSettings.flashes)
        {
            textureSpeedTokens.RestoreScale();
            return;
        }

        textureSpeedTokens.SetScale(0.1f);
    }

    public void ActivateSphere()
    {
        if (!upgradeInstalled) return;

        subRoot.voiceNotificationManager.PlayVoiceNotification(activationVoiceline);

        activationSfx.Play();
        Invoke(nameof(StartGrow), activationDelay);
        activating = true;
    }

    private void StartGrow()
    {
        currentSphereGrowTimeTime = 0;
        deployingLastFrame = false;
        activating = false;
        UWE.CoroutineHost.StartCoroutine(StartFreezeChecks());

        powerRelay.ConsumeEnergy(PrototypePowerSystem.CHARGE_POWER_AMOUNT * chargeConsumptionAmount, out _);
    }

    public override bool GetUpgradeEnabled() => upgradeInstalled;

    public override bool OnActivated()
    {
        if (currentSphereGrowTimeTime < sphereGrowTime || currentCooldownTime > 0)
        {
            return false;
        }

        if (activating) return false;
        
        if (powerRelay.GetPower() < PrototypePowerSystem.CHARGE_POWER_AMOUNT * chargeConsumptionAmount)
        {
            return false;
        }
        
        ActivateSphere();
        return true;
    }

    public override void OnSelectedChanged(bool changed) { }
}
