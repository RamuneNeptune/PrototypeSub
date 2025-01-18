using PrototypeSubMod.MiscMonobehaviors.Materials;
using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class DeployableLight : MonoBehaviour, IProtoEventListener
{
    [Header("Deployment")]
    [SerializeField] private PrefabIdentifier identifier;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float scaleSpeed;
    [SerializeField] private Light light;
    [SerializeField] private Collider sphereCollider;
    [SerializeField] private Collider[] halfColliders;
    [SerializeField] private float deployDelay;
    [SerializeField] private float lifetime;

    [Header("SFX")]
    [SerializeField] private FMOD_CustomEmitter deploySFX;
    [SerializeField] private FMOD_CustomLoopingEmitter loopingSFX;
    [SerializeField] private FMOD_CustomEmitter breakSFX;

    [Header("Breaking (Bad)")]
    [SerializeField] private GameObject topHalf;
    [SerializeField] private GameObject bottomHalf;
    [SerializeField] private GameObject lightVisual;

    private float currentLifetime;
    private float lightRange;
    private Vector3 volumetricSize;
    private Pickupable pickupable;
    private EcoTarget ecoTarget;

    private bool piecesSeparated;
    private bool activated;

    private void Awake()
    {
        sphereCollider.enabled = false;

        lightRange = light.range;
        volumetricSize = light.transform.localScale;

        light.range = 0;
        light.transform.localScale = Vector3.zero;
        pickupable = GetComponent<Pickupable>();
        ecoTarget = GetComponent<EcoTarget>();
        ecoTarget.enabled = false;
    }

    public void LaunchWithForce(float force, Vector3 previousVelocity)
    {
        sphereCollider.enabled = false;

        rb.AddForce((transform.forward * force) + previousVelocity, ForceMode.Impulse);

        Invoke(nameof(ActivateLight), deployDelay);
        Destroy(pickupable);

        foreach (var item in halfColliders)
        {
            item.enabled = false;
        }
    }

    private void OnDrop()
    {
        Invoke(nameof(ActivateLight), deployDelay);
        Destroy(pickupable);
    }

    private void ActivateLight()
    {
        activated = true;
        ecoTarget.enabled = true;
        animator.SetTrigger("Activate");
        deploySFX.Play();

        foreach (var item in halfColliders)
        {
            item.enabled = true;
        }
    }

    private void Update()
    {
        if (currentLifetime < lifetime)
        {
            currentLifetime += Time.deltaTime;
        }
        else if (!piecesSeparated)
        {
            BreakLight();
        }

        if (currentLifetime >= 0.25f)
        {
            sphereCollider.enabled = true;
        }

        if (!activated) return;

        float targetRange = activated && !piecesSeparated ? lightRange : 0;
        Vector3 targetScale = activated && !piecesSeparated ? volumetricSize : Vector3.zero;

        light.range = Mathf.Lerp(light.range, targetRange, Time.deltaTime * scaleSpeed);
        light.transform.localScale = Vector3.Lerp(light.transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

        if (light.transform.localScale.magnitude > targetScale.magnitude - 0.5f && !piecesSeparated)
        {
            loopingSFX.Play();
        }

        if (light.range < 1e-5)
        {
            light.range = 0;
            light.enabled = false;
        }
        else
        {
            light.enabled = true;
        }
    }

    private void BreakLight()
    {
        piecesSeparated = true;

        topHalf.transform.SetParent(null);
        bottomHalf.transform.SetParent(null);
        lightVisual.SetActive(false);

        var rb1 = topHalf.AddComponent<Rigidbody>();
        var rb2 = bottomHalf.AddComponent<Rigidbody>();

        rb1.interpolation = RigidbodyInterpolation.Interpolate;
        rb2.interpolation = RigidbodyInterpolation.Interpolate;

        rb1.AddForce((Random.onUnitSphere * 5f) + (topHalf.transform.forward * 10f));
        rb2.AddForce((Random.onUnitSphere * 5f) - (bottomHalf.transform.forward * 10f));

        GetComponentInChildren<Animator>().enabled = false;

        Destroy(topHalf, 10f);
        Destroy(bottomHalf, 10f);
        Destroy(GetComponent<PrefabIdentifier>());

        breakSFX.Play();
        loopingSFX.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        ecoTarget.enabled = false;
    }

    public void OnProtoSerialize(ProtobufSerializer serializer)
    {
        if (piecesSeparated)
        {
            Destroy(gameObject);
        }

        if (Plugin.GlobalSaveData.deployableLightLifetimes.ContainsKey(identifier.Id))
        {
            Plugin.GlobalSaveData.deployableLightLifetimes[identifier.Id] = currentLifetime;
        }
        else
        {
            Plugin.GlobalSaveData.deployableLightLifetimes.Add(identifier.Id, currentLifetime);
        }
    }

    public void OnProtoDeserialize(ProtobufSerializer serializer)
    {
        currentLifetime = Plugin.GlobalSaveData.deployableLightLifetimes[identifier.Id];
        ActivateLight();
    }
}
