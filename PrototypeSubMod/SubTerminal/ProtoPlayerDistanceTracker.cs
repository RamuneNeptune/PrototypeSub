using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace PrototypeSubMod.SubTerminal;

internal class ProtoPlayerDistanceTracker : MonoBehaviour, IScheduledUpdateBehaviour, IManagedBehaviour
{
    public float SqrDistanceToPlayer
    {
        get
        {
            return _sqrDistanceToPlayer;
        }
    }

    private float _sqrDistanceToPlayer;

    public event Action<bool> OnPlayerTriggerChanged;

    [SerializeField] private float maxDistance;
    [SerializeField] private float triggerDistance;
    [SerializeField] private float timeBetweenUpdates = 0.4f;

    private float timeLastUpdate;
    private bool playerInTriggerLastCheck;

    public int scheduledUpdateIndex { get; set; }

    public string GetProfileTag()
    {
        return "ProtoPlayerDistanceTracker";
    }

    public void ScheduledUpdate()
    {
        if (timeLastUpdate + timeBetweenUpdates >= Time.time)
        {
            return;
        }

        float sqrMagnitude = (transform.position - Player.main.transform.position).sqrMagnitude;
        bool nearby = sqrMagnitude < maxDistance * maxDistance;
        _sqrDistanceToPlayer = nearby ? sqrMagnitude : float.PositiveInfinity;

        bool inTrigger = sqrMagnitude <= (triggerDistance * triggerDistance);
        if (inTrigger != playerInTriggerLastCheck)
        {
            OnPlayerTriggerChanged?.Invoke(inTrigger);
        }

        timeLastUpdate += timeBetweenUpdates;
        playerInTriggerLastCheck = inTrigger;
    }

    private void Start()
    {
        timeLastUpdate = Time.time - (Random.value * timeBetweenUpdates);
    }

    private void OnEnable()
    {
        timeLastUpdate = Time.time - (Random.value * timeBetweenUpdates);
        UpdateSchedulerUtils.Register(this);
    }

    private void OnDisable()
    {
        UpdateSchedulerUtils.Deregister(this);
    }

    private void OnDestroy()
    {
        UpdateSchedulerUtils.Deregister(this);
    }
}
