using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class DeployableLight : MonoBehaviour, IProtoEventListener
{
    [Header("Launching")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float scaleSpeed;
    [SerializeField] private Light light;
    [SerializeField] private Collider sphereCollider;

    [Header("SFX")]
    [SerializeField] private FMOD_CustomEmitter deploySFX;
    [SerializeField] private FMOD_CustomLoopingEmitter loopingSFX;
    [SerializeField] private FMOD_CustomEmitter breakSFX;

    [Header("Breaking (Bad)")]
    [SerializeField] private GameObject topHalf;
    [SerializeField] private GameObject bottomHalf;
    [SerializeField] private GameObject lightVisual;

    private float lifetime;
    private float currentLifetime;
    private float lightRange;
    private Vector3 volumetricSize;
    private Pickupable pickupable;

    private bool piecesSeparated;
    private bool activated;

    private void Start()
    {
        lightRange = light.range;
        volumetricSize = light.transform.localScale;

        light.range = 0;
        light.transform.localScale = Vector3.zero;
        pickupable = GetComponent<Pickupable>();
    }

    public void LaunchWithForce(float force, Vector3 previousVelocity, float lifetime, float delay)
    {
        sphereCollider.enabled = false;

        rb.AddForce((transform.forward * force) + previousVelocity, ForceMode.Impulse);

        this.lifetime = lifetime;
        Invoke(nameof(StartLifetime), delay);
    }

    private void OnDrop()
    {
        StartLifetime();
    }

    private void StartLifetime()
    {
        activated = true;
        animator.SetTrigger("Activate");
        deploySFX.Play();

        Destroy(pickupable);
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
    }

    public void OnProtoSerialize(ProtobufSerializer serializer)
    {
        if (piecesSeparated)
        {
            Destroy(gameObject);
        }
    }

    public void OnProtoDeserialize(ProtobufSerializer serializer) { }
}
