using PrototypeSubMod.PathCreation;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

public class FollowBezierPath : MonoBehaviour
{
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private float speed;

    private float traveledDistance;
    
    private void Update()
    {
        traveledDistance += speed * Time.deltaTime;
        
        transform.position = pathCreator.path.GetPointAtDistance(traveledDistance);
        transform.rotation = pathCreator.path.GetRotationAtDistance(traveledDistance);
    }
}