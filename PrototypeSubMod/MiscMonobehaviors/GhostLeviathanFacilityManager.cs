using System;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

public class GhostLeviathanFacilityManager : MonoBehaviour
{
    private Locomotion locomotion;
    private Rigidbody rigidbody;
    private bool wasInHullFacility;

    private void Start()
    {
        locomotion = GetComponent<Locomotion>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnUpdate()
    {
        var biomeString = AtmosphereDirector.main.GetBiomeOverride();
        bool inHullFacility = biomeString == "protohullfacilitycalm" || biomeString == "protohullfacilitytense";

        if (inHullFacility != wasInHullFacility)
        {
            UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, inHullFacility);
            locomotion.enabled = !inHullFacility;
        }

        wasInHullFacility = inHullFacility;
    }

    private void OnEnable()
    {
        ManagedUpdate.Subscribe(ManagedUpdate.Queue.Update, OnUpdate);
    }

    private void OnDisable()
    {
        ManagedUpdate.Unsubscribe(ManagedUpdate.Queue.Update, OnUpdate);
    }
}