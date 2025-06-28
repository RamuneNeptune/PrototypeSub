﻿using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class FinFoldManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float sphereRadius;
    [SerializeField] private float checkDistance;
    [SerializeField] private float minMassForFold = 100;
    [SerializeField] private int gizmoStepLength;

    private ProtoFinsManager manager;
    private RaycastHit[] hitInfos;
    private int layerMask;
    private bool hadHitObject;

    private void Awake()
    {
        manager = GetComponentInParent<ProtoFinsManager>();
        hitInfos = new RaycastHit[20];

        layerMask = int.MaxValue;
        layerMask &= ~(1 << LayerID.Vehicle);
        layerMask &= ~(1 << LayerID.Player);
        layerMask &= ~(1 << LayerID.Trigger);
    }

    private void FixedUpdate()
    {
        var ray = new Ray(transform.position, transform.forward);
        int hitCount = Physics.SphereCastNonAlloc(ray, sphereRadius, hitInfos, checkDistance, layerMask);

        bool hitObject = false;
        for (int i = 0; i < hitCount; i++)
        {
            var hitInfo = hitInfos[i];
            if (!LayerID.IsMaskContainsLayer(layerMask, hitInfo.collider.gameObject.layer)) continue;

            if (hitInfo.rigidbody != null && hitInfo.rigidbody.mass < minMassForFold) continue;
            
            hitObject = true;
            break;
        }

        if (hitObject != hadHitObject)
        {
            animator.SetBool("CrampedFold", hitObject);
            if (!hitObject)
            {
                manager.ResetFinAnimations();
            }
        }
        
        hadHitObject = hitObject;
    }
    
    private void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        Gizmos.color = Color.cyan;
        float step = checkDistance / (gizmoStepLength - 1);
        for (int i = 0; i < gizmoStepLength; i++)
        {
            Gizmos.DrawWireSphere(pos, sphereRadius);
            pos += transform.forward * step;
        }
    }
}