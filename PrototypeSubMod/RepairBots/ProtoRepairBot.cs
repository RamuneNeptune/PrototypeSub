using PrototypeSubMod.Pathfinding;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class ProtoRepairBot : PathfindingObject
{
    [SerializeField] private GameObject placeholderGraphic;

    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        animator.SetBool(AnimatorHashID.on_ground, true);

        placeholderGraphic.SetActive(false);
        base.OnPathFinished += OnPathFinished;
    }

    private void Update()
    {
        if (path == null) return;

        Vector3 posOnPlane = Vector3.ProjectOnPlane(directionToNextPoint + visual.position, lastNormal);
        posOnPlane += visual.position;

        Vector3 dir = (posOnPlane - visual.position).normalized;
        Vector3 localDir = visual.InverseTransformDirection(dir);

        animator.SetFloat(AnimatorHashID.move_speed_x, localDir.x);
        animator.SetFloat(AnimatorHashID.move_speed_y, localDir.y);
        animator.SetFloat(AnimatorHashID.speed, localDir.magnitude);
    }

    new private void OnPathFinished()
    {
        animator.SetFloat(AnimatorHashID.move_speed_x, 0);
        animator.SetFloat(AnimatorHashID.move_speed_y, 0);
        animator.SetFloat(AnimatorHashID.speed, 0);
    }
}
