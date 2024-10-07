using Nautilus.Extensions;
using PrototypeSubMod.Pathfinding;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class ProtoRepairBot : PathfindingObject
{
    [SerializeField] private GameObject placeholderGraphic;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private Transform welderFXSpawnPos;
    [SerializeField] private FMOD_CustomLoopingEmitter repairSFX;
    [SerializeField] private float repairSpeed;

    private CyclopsDamagePoint targetPoint;
    private ProtoBotBay ownerBay;
    private Animator animator;
    private FMOD_CustomLoopingEmitter walkLoopEmitter;
    private VFXController welderController;

    private bool enRouteToPoint;
    private bool repairing;
    private bool vfxEnabled;

    private void Start()
    {
        var fxController = Plugin.welderPrefab.transform.Find("SparkEmit");
        welderController = Instantiate(fxController, welderFXSpawnPos).GetComponent<VFXController>();
        welderController.Stop();
        welderController.transform.localPosition = Vector3.zero;

        animator = GetComponentInChildren<Animator>();
        animator.SetBool(AnimatorHashID.on_ground, true);

        placeholderGraphic.SetActive(false);
        base.OnPathFinished += OnPathFinished;

        walkLoopEmitter = GetComponentInChildren<FMOD_CustomLoopingEmitter>();
        walkLoopEmitter.Stop();
    }

    private void Update()
    {
        HandleRepairing();
        HandleMovementAnims();
    }

    private void HandleRepairing()
    {
        if (!repairing) return;

        welderController.transform.LookAt(Camera.main.transform.position);
        welderController.transform.rotation = Quaternion.Inverse(welderController.transform.rotation);

        if (!vfxEnabled)
        {
            welderController.Play();
            vfxEnabled = true;
        }
        
        repairSFX.Play();

        targetPoint.liveMixin.AddHealth(repairSpeed * Time.deltaTime);
        if (targetPoint.liveMixin.GetHealthFraction() >= 1)
        {
            repairing = false;
            vfxEnabled = false;
            ownerBay.OnPointRepaired();
            welderController.Stop();
            repairSFX.Stop();
        }
    }

    private void HandleMovementAnims()
    {
        if (path == null) return;

        animator.enabled = true;
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
        animator.enabled = false;
        
        walkLoopEmitter.Stop();

        if (targetPoint != null)
        {
            visual.rotation = Quaternion.LookRotation(targetPoint.transform.position - visual.position, targetPoint.transform.up);
        }

        if (enRouteToPoint)
        {
            enRouteToPoint = false;
            repairing = true;
        }
        else
        {
            // Back at elevator
            ownerBay.OnReturnToElevator();
        }
    }

    public void SetBotLocalPos()
    {
        visualTransform.GetChild(1).localPosition = new Vector3(0, 0.2f, 0);
    }

    public void UpdateUseLocalPos()
    {
        grid = GetComponentInParent<PathfindingGrid>();
        useLocalPos = grid != null;
    }

    public void SetEnRouteToPoint()
    {
        enRouteToPoint = true;
    }

    public void SetOwnerBay(ProtoBotBay bay)
    {
        ownerBay = bay;
    }

    public void SetTargetPoint(CyclopsDamagePoint point)
    {
        targetPoint = point;
    }

    public void ResetVisualRotation()
    {
        visual.transform.localRotation = Quaternion.identity;
    }
}
