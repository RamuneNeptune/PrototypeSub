using PrototypeSubMod.Upgrades;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.StasisPulse;

internal class ProtoStasisPulse : ProtoUpgrade
{
    [SerializeField] private AnimationCurve sphereRadius;
    [SerializeField] private Gradient colorOverLifetime;
    [SerializeField] private PowerRelay powerRelay;
    [SerializeField] private VoiceNotification activationVoiceline;
    [SerializeField] private float activationDelay;
    [SerializeField] private float powerCost;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float sphereGrowTime;
    [SerializeField] private float minFreezeTime;
    [SerializeField] private float maxFreezeTime;
    [SerializeField] private Renderer sphereVisual;

    private float CurrentDiameter
    {
        get
        {
            return sphereRadius.Evaluate(currentSphereGrowTimeTime / sphereGrowTime);
        }
    }

    private List<FlashingLightHelpers.ShaderVector4ScalerToken> textureSpeedTokens;
    private GameObject freezeFX;
    private GameObject unfreezeFX;
    private SubRoot subRoot;

    private float currentCooldownTime;
    private float currentSphereGrowTimeTime;
    private bool deployingLastFrame;

    private IEnumerator Start()
    {
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

        MiscSettings.isFlashesEnabled.changedEvent.AddHandler(this, new Event<Utils.MonitoredValue<bool>>.HandleFunction(OnFlashesEnabledChanged));
        UpdateTextureSpeed();

        currentSphereGrowTimeTime = sphereGrowTime;
        sphereVisual.enabled = false;

        subRoot = GetComponentInParent<SubRoot>();
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
        HandleFreezing();
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

    private void HandleFreezing()
    {
        if (currentSphereGrowTimeTime >= sphereGrowTime) return;

        int colliderCount = UWE.Utils.OverlapSphereIntoSharedBuffer(sphereVisual.transform.position, CurrentDiameter / 2f);
        for (int i = 0; i < colliderCount; i++)
        {
            Collider collider = UWE.Utils.sharedColliderBuffer[i];
            TryFreeze(collider);
        }
    }

    private bool TryFreeze(Collider collider)
    {
        Rigidbody rigidbody = collider.GetComponentInParent<Rigidbody>();
        if (!rigidbody) return false;

        if (rigidbody.GetComponentInChildren<ProtoStasisPulse>() != null) return false;

        if (rigidbody.TryGetComponent<ProtoStasisFreeze>(out var stasisFreeze)) return false;

        if (rigidbody.isKinematic) return false;

        if (collider.GetComponentInParent<Player>() != null) return false;

        var freeze = rigidbody.gameObject.AddComponent<ProtoStasisFreeze>();
        freeze.SetFreezeTimes(minFreezeTime, maxFreezeTime);
        freeze.SetUnfreezeVF(unfreezeFX);

        Utils.PlayOneShotPS(freezeFX, rigidbody.transform.position, Quaternion.identity);
        return true;
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

        if (currentSphereGrowTimeTime < sphereGrowTime || currentCooldownTime > 0)
        {
            return;
        }

        if (powerRelay.GetPower() < powerCost)
        {
            return;
        }

        subRoot.voiceNotificationManager.PlayVoiceNotification(activationVoiceline);

        Invoke(nameof(StartGrow), activationDelay);
    }

    private void StartGrow()
    {
        currentSphereGrowTimeTime = 0;
        deployingLastFrame = false;

        powerRelay.ConsumeEnergy(powerCost, out _);
    }

    public override bool GetUpgradeEnabled() => upgradeInstalled;

    public override void OnActivated()
    {
        ActivateSphere();
    }

    public override void OnSelectedChanged(bool changed) { }
}
