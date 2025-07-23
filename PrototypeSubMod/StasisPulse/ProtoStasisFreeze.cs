using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.StasisPulse;

internal class ProtoStasisFreeze : MonoBehaviour
{
    private const float MAX_MASS_VALUE = 200f;

    public bool isFrozen
    {
        get
        {
            return rigidbody.isKinematic;
        }
    }

    private Rigidbody rigidbody;
    private GameObject unfreezeFX;

    private float currentFreezeTime;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, true);
        rigidbody.SendMessage("OnFreezeByStasisSphere", SendMessageOptions.DontRequireReceiver);
        UWE.CoroutineHost.StartCoroutine(TakeDamageOverTime(GetComponent<LiveMixin>(), 10, 10));
    }

    private void Update()
    {
        if (currentFreezeTime > 0)
        {
            currentFreezeTime -= Time.deltaTime;
            return;
        }

        UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, false);
        rigidbody.SendMessage("OnUnfreezeByStasisSphere", SendMessageOptions.DontRequireReceiver);
        Utils.PlayOneShotPS(unfreezeFX, transform.position, Quaternion.identity);

        Destroy(this);
    }

    private IEnumerator TakeDamageOverTime(LiveMixin mixin, float duration, float damage)
    {
        float startTime = Time.time;
        float dmgPerUpdate = damage / duration;
        while (Time.time < startTime + duration)
        {
            mixin.TakeDamage(dmgPerUpdate * Time.deltaTime);
            yield return null;
        }
    }

    public void SetFreezeTimes(float minFreezeTime, float maxFreezeTime)
    {
        float normalizedMass = Mathf.InverseLerp(0, MAX_MASS_VALUE, rigidbody.mass);
        currentFreezeTime = Mathf.Lerp(maxFreezeTime, minFreezeTime, normalizedMass);
    }

    public void SetUnfreezeVF(GameObject unfreezeFX)
    {
        this.unfreezeFX = unfreezeFX;
    }
}
