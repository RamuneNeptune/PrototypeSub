using PrototypeSubMod.Interfaces;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.StasisPulse;

public class ProtoStasisPulse : MonoBehaviour, IProtoUpgrade
{
    [SerializeField] private AnimationCurve sphereRadius;
    [SerializeField] private Gradient colorOverLifetime;
    [SerializeField] private PowerRelay powerRelay;
    [SerializeField] private float powerCost;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float sphereGrowTime;
    [SerializeField] private float minFreezeTime;
    [SerializeField] private float maxFreezeTime;
    [SerializeField] private Renderer sphereVisual;

    private float currentCooldownTime;
    private float currentSphereGrowTimeTime;
    private bool upgradeActive;
    private bool deployingLastFrame;

    private IEnumerator Start()
    {
        var rifleTask = CraftData.GetPrefabForTechTypeAsync(TechType.StasisRifle);
        yield return rifleTask;

        var stasisRifle = rifleTask.GetResult();
        var stasisSphere = stasisRifle.GetComponent<StasisRifle>().effectSpherePrefab;

        var stasisMaterials = stasisSphere.GetComponent<Renderer>().materials;
        Material[] newMaterials = new Material[stasisMaterials.Length];

        for (int i = 0; i < stasisMaterials.Length; i++)
        {
            newMaterials[i] = Instantiate(stasisMaterials[i]);
        }

        sphereVisual.materials = newMaterials;
        sphereVisual.GetComponent<MeshFilter>().mesh = stasisSphere.GetComponent<MeshFilter>().mesh;

        currentSphereGrowTimeTime = sphereGrowTime;
        sphereVisual.enabled = false;
    }

    private void LateUpdate()
    {
        sphereVisual.enabled = currentSphereGrowTimeTime < sphereGrowTime;
        UpdateMaterials();

        if (!upgradeActive)
        {
            return;
        }

        if(currentCooldownTime > 0)
        {
            currentCooldownTime -= Time.deltaTime;
            return;
        }

        if (currentSphereGrowTimeTime < sphereGrowTime)
        {
            currentSphereGrowTimeTime += Time.deltaTime;
            float targetRadius = sphereRadius.Evaluate(currentSphereGrowTimeTime / sphereGrowTime);

            sphereVisual.transform.localScale = Vector3.one * targetRadius;
        }
        else if (deployingLastFrame)
        {
            currentCooldownTime = cooldownTime;
        }

        deployingLastFrame = currentSphereGrowTimeTime < sphereGrowTime;
    }

    private void UpdateMaterials()
    {
        Color color = colorOverLifetime.Evaluate(currentSphereGrowTimeTime / sphereGrowTime);
        sphereVisual.materials[0].SetColor(ShaderPropertyID._Color, color);
        sphereVisual.materials[1].SetColor(ShaderPropertyID._Color, color);
    }

    public void TryActivateSphere()
    {
        currentSphereGrowTimeTime = 0;
        deployingLastFrame = false;

        powerRelay.ConsumeEnergy(powerCost, out _);
    }

    public bool CanActivate()
    {
        return currentCooldownTime > 0;
    }

    public void SetUpgradeActive(bool active)
    {
        upgradeActive = active;
    }

    public bool GetUpgradeActive()
    {
        return upgradeActive;
    }
}
