using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class FloodlightsCameraTargeter : MonoBehaviour
{
    [SerializeField] private PilotingChair chair;
    [SerializeField] private float lookSpeed = 2;
    [SerializeField] private Vector2 yawMinMax;
    [SerializeField] private Vector2 pitchMinMax;
    
    private void Update()
    {
        Vector3 point = transform.position + chair.subRoot.transform.forward;
        var main = Camera.main;
        if (Player.main.currChair == chair)
        {
            const float maxDist = 50;
            
            bool hitTerrain = Physics.Raycast(main.transform.position, main.transform.forward,
                out RaycastHit hit, maxDist, 1 << LayerID.TerrainCollider);
            point = hit.point;
            if (!hitTerrain)
            {
                point = main.transform.position + main.transform.forward * maxDist;
            }
        }

        Vector3 previousRot = transform.localRotation.eulerAngles;
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(point - main.transform.position), lookSpeed * Time.deltaTime);
        Vector3 eulerAngles = transform.localRotation.eulerAngles;
        Vector3 checkAngles = Vector3.zero;
        checkAngles.x = eulerAngles.x > 180 ? eulerAngles.x - 360 : eulerAngles.x;
        checkAngles.y = eulerAngles.y > 180 ? eulerAngles.y - 360 : eulerAngles.y;
        float xOffset = eulerAngles.x > 180 ? 360 : 0;
        float yOffset = eulerAngles.y > 180 ? 360 : 0;

        transform.localRotation = Quaternion.Euler(
            Mathf.Clamp(checkAngles.x, pitchMinMax.x, pitchMinMax.y) + xOffset, 
            Mathf.Clamp(checkAngles.y, yawMinMax.x, yawMinMax.y) + yOffset, 
            eulerAngles.z);
    }
}