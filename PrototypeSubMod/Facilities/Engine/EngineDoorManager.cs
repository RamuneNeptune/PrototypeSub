using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Engine;

public class EngineDoorManager : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private IEnumerator Start()
    {
        rb.isKinematic = true;
        yield return new WaitUntil(() => LargeWorldStreamer.main.IsWorldSettled());
        
        rb.isKinematic = false;
    }
}