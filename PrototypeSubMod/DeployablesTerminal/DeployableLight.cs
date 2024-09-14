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

            topHalf.transform.SetParent(transform);
            bottomHalf.transform.SetParent(transform);

            topHalf.AddComponent<Rigidbody>(); 
            bottomHalf.AddComponent<Rigidbody>();
            GetComponentInChildren<Animator>().enabled = false;

            Destroy(GetComponent<PrefabIdentifier>());
        }

        float targetRange = activated && !piecesSeparated ? lightRange : 0;
        Vector3 targetScale = activated && !piecesSeparated ? volumetricSize : Vector3.zero;

        light.range = Mathf.Lerp(light.range, targetRange, Time.deltaTime * scaleSpeed);
        light.transform.localScale = Vector3.Lerp(light.transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }
}
