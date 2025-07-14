using System.Collections;
using PrototypeSubMod.Pathfinding;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class ProtoRepairBot : PathfindingObject
{
    private const string LEFT_ANT_PATH = "Visual/Precursor_Droid(Clone)/models/Precursor_Driod/Root/antennae_l1/antennae_l2/antennae_l3";
    private const string RIGHT_ANT_PATH = "Visual/Precursor_Droid(Clone)/models/Precursor_Driod/Root/antennae_r1/antennae_r2/antennae_r3";

    [SerializeField] private GameObject placeholderGraphic;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private Transform welderFXSpawnPos;
    [SerializeField] private FMOD_CustomLoopingEmitter repairSFX;
    [SerializeField] private FMOD_CustomEmitter returnToBaySfx;
    [SerializeField] private LineRenderer leftLineRend;
    [SerializeField] private LineRenderer rightLineRend;
    [SerializeField] private float repairSpeed;

    private CyclopsDamagePoint targetPoint;
    private ProtoBotBay ownerBay;
    private Animator animator;
    private FMOD_CustomLoopingEmitter walkLoopEmitter;
    private VFXController welderController;
    private Transform leftAntenna;
    private Transform rightAntenna;

    private bool initialized;
    private bool enRouteToPoint;
    private bool repairing;
    private bool vfxEnabled;

    private void Awake()
    {
        UWE.CoroutineHost.StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitUntil(() => Plugin.welderPrefab);
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
        leftLineRend.enabled = false;
        rightLineRend.enabled = false;

        leftAntenna = transform.Find(LEFT_ANT_PATH);
        rightAntenna = transform.Find(RIGHT_ANT_PATH);
        
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;
        
        if (!targetPoint && enRouteToPoint && !repairing)
        {
            repairing = false;
            vfxEnabled = false;
            ownerBay.OnPointRepaired();
            welderController.Stop();
            repairSFX.Stop();
            return;
        }
        
        HandleRepairing();
        HandleMovementAnims();
    }

    private void HandleRepairing()
    {
        leftLineRend.enabled = repairing;
        rightLineRend.enabled = repairing;

        if (!repairing) return;

        leftLineRend.SetPositions(new Vector3[] { leftAntenna.position, targetPoint.transform.position });
        rightLineRend.SetPositions(new Vector3[] { rightAntenna.position, targetPoint.transform.position });

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

        animator.SetFloat(AnimatorHashID.move_speed_x, 0);
        animator.SetFloat(AnimatorHashID.move_speed_z, 1);
        animator.SetFloat(AnimatorHashID.speed, 1);
    }

    new private void OnPathFinished()
    {
        animator.SetFloat(AnimatorHashID.move_speed_x, 0);
        animator.SetFloat(AnimatorHashID.move_speed_z, 0);
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

    public void PlayReturnToBaySfx()
    {
        returnToBaySfx.Play();
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

    public void SetRepairPoint(CyclopsDamagePoint point)
    {
        targetPoint = point;
    }

    public void ResetVisualRotation()
    {
        visual.transform.localRotation = Quaternion.identity;
    }
}
