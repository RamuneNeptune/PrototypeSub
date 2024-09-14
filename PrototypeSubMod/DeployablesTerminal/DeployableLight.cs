using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class DeployableLight : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float scaleSpeed;
    [SerializeField] private Light light;

    [SerializeField] private GameObject topHalf;
    [SerializeField] private GameObject bottomHalf;
    [SerializeField] private GameObject lightVisual;

    private float lifetime;
    private float currentLifetime;
    private float lightRange;
    private Vector3 volumetricSize;

    private bool piecesSeparated;
    private bool activated;

    private void Start()
    {
        lightRange = light.range;
        volumetricSize = light.transform.localScale;

        light.range = 0;
        light.transform.localScale = Vector3.zero;
    }

    public void LaunchWithForce(float force, Vector3 previousVelocity, float lifetime, float delay)
    {
        rb.AddForce((transform.forward * force) + previousVelocity, ForceMode.Impulse);

        this.lifetime = lifetime;
        Invoke(nameof(StartLifetime), delay);
    }

    private void StartLifetime()
    {
        activated = true;
        animator.SetTrigger("Activate");
    }

    private void Update()
    {
        if (currentLifetime < lifetime)
        {
            currentLifetime += Time.deltaTime;
        }
        else if (!piecesSeparated)
        {
            piecesSeparated = true;

            topHalf.transform.SetParent(null);
            bottomHalf.transform.SetParent(null);
            lightVisual.SetActive(false);

            var rb1 = topHalf.AddComponent<Rigidbody>();
            var rb2 = bottomHalf.AddComponent<Rigidbody>();

            rb1.interpolation = RigidbodyInterpolation.Interpolate;
            rb2.interpolation = RigidbodyInterpolation.Interpolate;

            rb1.AddForce(Random.onUnitSphere * 5f);
            rb2.AddForce(Random.onUnitSphere * 5f);

            GetComponentInChildren<Animator>().enabled = false;

            Destroy(topHalf, 10f);
            Destroy(bottomHalf, 10f);
            Destroy(GetComponent<PrefabIdentifier>());
        }

        float targetRange = activated && !piecesSeparated ? lightRange : 0;
        Vector3 targetScale = activated && !piecesSeparated ? volumetricSize : Vector3.zero;

        light.range = Mathf.Lerp(light.range, targetRange, Time.deltaTime * scaleSpeed);
        light.transform.localScale = Vector3.Lerp(light.transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }
}
