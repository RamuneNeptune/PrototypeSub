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

    private float currentFreezeTime;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, true);
        rigidbody.SendMessage("OnFreezeByStasisSphere", SendMessageOptions.DontRequireReceiver);
    }

    private void Update()
    {
        if(currentFreezeTime > 0)
        {
            currentFreezeTime -= Time.deltaTime;
            return;
        }

        UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, false);
        rigidbody.SendMessage("OnUnfreezeByStasisSphere", SendMessageOptions.DontRequireReceiver);
    }

    public void SetFreezeTimes(float minFreezeTime, float maxFreezeTime)
    {
        float normalizedMass = Mathf.InverseLerp(0, MAX_MASS_VALUE, rigidbody.maxAngularVelocity);
        currentFreezeTime = Mathf.Lerp(minFreezeTime, maxFreezeTime, normalizedMass);
    }
}
