using System;
using SubLibrary.Materials.Tags;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class SubLODManager : MonoBehaviour, IScheduledUpdateBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject interiorObjectsHolder;
    [SerializeField] private ProtoWindowManager windowManager;
    [SerializeField] private float disableDistance = 100;
    
    private void Start()
    {
        foreach (var rend in model.GetComponentsInChildren<Renderer>(true))
        {
            if (rend.GetComponent<SubExteriorTag>()) continue;
            if (rend.GetComponent<ProtoLODBlacklist>()) continue;
            
            rend.transform.SetParent(interiorObjectsHolder.transform, true);
        }
    }

    public string GetProfileTag() => "ProtoSubLODManager";

    public void ScheduledUpdate()
    {
        bool inRange = (Camera.main.transform.position - transform.position).sqrMagnitude <
                       (disableDistance * disableDistance);
        interiorObjectsHolder.gameObject.SetActive(inRange);
    }

    public int scheduledUpdateIndex { get; set; }
    
    public virtual void OnEnable()
    {
        UpdateSchedulerUtils.Register(this);
    }

    public virtual void OnDisable()
    {
        UpdateSchedulerUtils.Deregister(this);
    }
}