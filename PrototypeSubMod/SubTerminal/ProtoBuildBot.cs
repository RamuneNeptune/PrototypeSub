using System;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

internal class ProtoBuildBot : MonoBehaviour
{
    private static Material LineRendMaterial;

    [SerializeField] private Transform botBone;
    [SerializeField] private Transform parentObj;
    [SerializeField] private Transform originalParentObj;
    [SerializeField] private GameObject hoverSoundsObject;
    [SerializeField] private FMOD_CustomEmitter buildLoopingSound;
    [SerializeField] private Transform beamOrigin;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 1f;

    private Action onReturnedToStart;
    private BuildBotPath currentPath;
    private GameObject objectToBuild;
    private FMOD_CustomLoopingEmitter hoverEmitter;
    private LineRenderer lineRend;
    private Transform currentBeamPoint;
    private Vector3 targetPos;
    private int currentPointIndex;
    private bool buildingSub;
    private bool animatorControlled = true;

    private Vector3 startPos;
    private Quaternion startRot;

    private void Start()
    {
        var hoverSounds = hoverSoundsObject.GetComponents<FMOD_CustomLoopingEmitter>();
        hoverEmitter = hoverSounds[UnityEngine.Random.Range(0, hoverSounds.Length - 1)];

        UWE.CoroutineHost.StartCoroutine(InitializeLineRend());

        buildingSub = false;
    }

    public void SetPath(BuildBotPath newPath, GameObject toConstruct)
    {
        transform.position = botBone.position - parentObj.localPosition;
        transform.rotation = botBone.rotation;
        botBone.SetParent(parentObj);
        botBone.transform.localPosition = Vector3.zero;
        botBone.transform.localRotation = Quaternion.identity;

        startPos = transform.position;
        startRot = transform.rotation;

        currentPath = newPath;
        currentPointIndex = UnityEngine.Random.Range(0, newPath.points.Length - 1);
        targetPos = newPath.points[0].position;
        objectToBuild = toConstruct;
        buildingSub = true;
        animatorControlled = false;

        CancelInvoke(nameof(FindClosestBeamPoint));
        InvokeRepeating(nameof(FindClosestBeamPoint), 0, 0.3f);
    }

    private IEnumerator RetrieveBeamMaterial()
    {
        if (LineRendMaterial != null)
        {
            yield break;
        }

        var constructorTask = CraftData.GetPrefabForTechTypeAsync(TechType.Constructor);
        yield return constructorTask;

        var constructor = constructorTask.GetResult();
        constructor.gameObject.SetActive(false);
        var botPrefab = constructor.GetComponent<Constructor>().buildBotPrefab;

        LineRendMaterial = new Material(botPrefab.GetComponent<ConstructorBuildBot>().beamMaterial);
    }

    private IEnumerator InitializeLineRend()
    {
        yield return RetrieveBeamMaterial();

        lineRend = gameObject.AddComponent<LineRenderer>();
        lineRend.useWorldSpace = true;
        lineRend.positionCount = 2;
        lineRend.startWidth = 0.1f;
        lineRend.endWidth = 1;
        lineRend.startColor = new Color(0.42f, 1, 0.42f);
        lineRend.endColor = new Color(1f, 0.55f, 0.42f);
        lineRend.material = LineRendMaterial;
        lineRend.enabled = false;
    }

    private void Update()
    {
        if (lineRend == null) return;

        bool enableBeam = currentBeamPoint != null && (currentBeamPoint.position - transform.position).magnitude < 8f;
        lineRend.enabled = enableBeam;
        if (enableBeam)
        {
            lineRend.SetPosition(0, beamOrigin.position);
            lineRend.SetPosition(1, currentBeamPoint.position);
            buildLoopingSound.Play();
        }
        else
        {
            buildLoopingSound.Stop();
        }

        if (animatorControlled) return;

        Quaternion targetRotation = !buildingSub ? startRot : Quaternion.LookRotation(currentBeamPoint.position - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (animatorControlled) return;

        Vector3 targetPoint = buildingSub ? targetPos : startPos;
        if ((targetPoint - transform.position).sqrMagnitude > 1.6f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.deltaTime * movementSpeed);
        }
        else if (currentPath != null)
        {
            currentPointIndex++;
            if (currentPointIndex >= currentPath.points.Length)
            {
                currentPointIndex = 0;
            }
            targetPos = currentPath.points[currentPointIndex].position;
        }
        else if (!buildingSub)
        {
            OnReturnedToStart();
        }
    }

    private void FindClosestBeamPoint()
    {
        currentBeamPoint = null;
        if (objectToBuild == null)
        {
            buildingSub = false;
            return;
        }
        var beamPoints = objectToBuild.GetComponentInChildren<BuildBotBeamPoints>();
        if (beamPoints != null)
        {
            currentBeamPoint = beamPoints.GetClosestTransform(base.transform.position);
        }
    }

    public void FinishConstruction(Action onBotReturnedToStart)
    {
        buildingSub = false;
        objectToBuild = null;
        currentPath = null;
        currentBeamPoint = null;

        CancelInvoke(nameof(FindClosestBeamPoint));
        onReturnedToStart = onBotReturnedToStart;
    }

    private void OnReturnedToStart()
    {
        onReturnedToStart?.Invoke();
        animatorControlled = true;

        botBone.SetParent(originalParentObj, false);
    }
}
