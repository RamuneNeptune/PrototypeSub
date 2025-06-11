using System;
using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.MiscMonobehaviors;

public class TriggerEnterForward : MonoBehaviour
{
    [SerializeField] private GameObject[] targetObjects;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var obj in targetObjects)
        {
            obj.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (var obj in targetObjects)
        {
            obj.SendMessage("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
        }
    }
}