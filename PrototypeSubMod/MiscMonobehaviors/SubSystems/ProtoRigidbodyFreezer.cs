using System;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class ProtoRigidbodyFreezer : MonoBehaviour
{
    [SerializeField] private float freezeDistance = 48f;
    [SerializeField] private Transform[] colliderActivationStages;
    
    private Rigidbody rigidbody;
    private Coroutine updateCoroutine;
    private bool collidersTransitioning;
    private bool wasInDistance;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        bool inDistance = (MainCamera.camera.transform.position - transform.position).sqrMagnitude < freezeDistance * freezeDistance;
        if (inDistance != wasInDistance)
        {
            collidersTransitioning = true;
            if (collidersTransitioning && updateCoroutine != null)
            {
                UWE.CoroutineHost.StopCoroutine(updateCoroutine);
            }
            
            updateCoroutine = UWE.CoroutineHost.StartCoroutine(UpdateCollidersAndKinematics(inDistance));
        }

        wasInDistance = inDistance;
    }

    private IEnumerator UpdateCollidersAndKinematics(bool inDistance)
    {
        if (inDistance)
        {
            UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, false);
        }
        
        for (int i = 0; i < colliderActivationStages.Length; i++)
        {
            colliderActivationStages[i].gameObject.SetActive(inDistance);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }
        
        if (!inDistance)
        {
            UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, true);
        }
        
        collidersTransitioning = false;
    }
}