using PrototypeSubMod.Pathfinding;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class ProtoRepairBot : PathfindingObject
{
    [SerializeField] private GameObject placeholderGraphic;
    [SerializeField] private Transform visualTransform;

    private Animator animator;
    private FMOD_CustomLoopingEmitter walkLoopEmitter;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        animator.SetBool(AnimatorHashID.on_ground, true);

        placeholderGraphic.SetActive(false);
        base.OnPathFinished += OnPathFinished;

        walkLoopEmitter = GetComponentInChildren<FMOD_CustomLoopingEmitter>();
        walkLoopEmitter.Stop();
    }

    private void Update()
    {
        if (path == null) return;

        walkLoopEmitter.Start();
        Vector3 posOnPlane = Vector3.ProjectOnPlane(directionToNextPoint + visual.position, lastNormal);
        posOnPlane += visual.position;

        Vector3 dir = posOnPlane - visual.position;
        Vector3 localDir = visual.InverseTransformDirection(dir);

        animator.SetFloat(AnimatorHashID.move_speed_x, localDir.normalized.z);
        animator.SetFloat(AnimatorHashID.move_speed_y, localDir.normalized.x);
        animator.SetFloat(AnimatorHashID.speed, localDir.magnitude);
    }

    new private void OnPathFinished()
    {
        animator.SetFloat(AnimatorHashID.move_speed_x, 0);
        animator.SetFloat(AnimatorHashID.move_speed_y, 0);
        animator.SetFloat(AnimatorHashID.speed, 0);

        walkLoopEmitter.Stop();
    }

    public void SetBotLocalPos()
    {
        visualTransform.GetChild(0).localPosition = new Vector3(0, 0.2f, 0);
    }
}
