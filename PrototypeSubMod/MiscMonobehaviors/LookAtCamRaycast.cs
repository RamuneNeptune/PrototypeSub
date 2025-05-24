using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

public class LookAtCamRaycast : MonoBehaviour
{
    [SerializeField] private float lookSpeed = 2;
    
    private void Update()
    {
        const float maxDist = 50;
        bool hitTerrain = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
            out RaycastHit hit, maxDist, 1 << LayerID.TerrainCollider);
        Vector3 point = hit.point;
        if (!hitTerrain)
        {
            point = Camera.main.transform.position + Camera.main.transform.forward * maxDist;
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(point - Camera.main.transform.position), lookSpeed * Time.deltaTime);
    }
}