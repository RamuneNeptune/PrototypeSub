using System.Collections;
using FMODUnity;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.Prefabs;

public class AlienFabricator : GhostCrafter
{
    public Animator animator;
    
    public GameObject fxSparksPrefab;
    public GameObject customForceField;
    public GameObject squareForceField;
    
    public Transform soundOrigin;
    
    public FMODAsset openSound;
    public FMODAsset closeSound;

    public FMOD_CustomLoopingEmitter loopEmitter;
    

    public GameObject fabLight;

    public PowerRelay pRelay;

    private static readonly int isOpenAnimProperty = Animator.StringToHash("isOpen");

    private GameObject targetForcefield;
    private VFXLerpColor targetColorControl;
    
    private GameObject[] fxSparksInstances;

    private VFXLerpColor customColorControl;
    private VFXLerpColor squareColorControl;

    private float sparkFXOffset = 0.1f;
    
    private new void Awake()
    {
        base.Awake();
        
        craftTree = PrecursorFabricator.precursorFabricatorType;
        
        customColorControl = customForceField.GetComponent<VFXLerpColor>();
        squareColorControl = squareForceField.GetComponent<VFXLerpColor>();

        powerRelay = pRelay;
    }

    public override void Start()
    {
        base.Start();

        customForceField.SetActive(true);
        squareForceField.SetActive(false);
        
        if(fxSparksPrefab != null)
            InitSparkFX();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        if (logic != null && logic.inProgress && fxSparksPrefab)
        {
            for (int i = 0; i < fxSparksInstances.Length; i++)
            {
                var sparkInstance = fxSparksInstances[i];

                if (sparkInstance != null)
                {
                    var referenceTransform = ghost != null && ghost.itemSpawnPoint != null ? ghost.itemSpawnPoint : transform;
                    sparkInstance.transform.position = GetOffsetSparkPos(i, referenceTransform);
                }
            }
        }
    }

    public override void OnStateChanged(bool crafting)
    {
        animator.SetBool(AnimatorHashID.fabricating, crafting);

        if (loopEmitter.playing != crafting)
        {
            if(crafting)
                loopEmitter.Play();
            else
                loopEmitter.Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        
        if(fxSparksInstances == null)
            InitSparkFX();

        for (int i = 0; i < fxSparksInstances.Length; i++)
        {
            var sparkInstance = fxSparksInstances[i];

            if (sparkInstance != null)
            {
                var particleSystem = sparkInstance.GetComponent<ParticleSystem>();

                if (particleSystem != null)
                {
                    if(crafting)
                        particleSystem.Play();
                    else
                        particleSystem.Stop();
                }
            }
        }
        
        fabLight.SetActive(crafting);
    }

    public override void Craft(TechType techType, float duration)
    {
        CoroutineHost.StartCoroutine(CraftItem(techType, duration));
    }

    private IEnumerator CraftItem(TechType techType, float duration)
    {
        animator.SetBool(AnimatorHashID.fabricating, true);
        FMODUWE.PlayOneShot(closeSound, soundOrigin.position);
        yield return new WaitForSeconds(1.50f);
        base.Craft(techType, duration);
        yield return new WaitForSeconds(duration);
        FMODUWE.PlayOneShot(openSound, soundOrigin.position);
    }

    public override void OnOpenedChanged(bool opened)
    {
        base.OnOpenedChanged(opened);
        
        animator.SetBool(isOpenAnimProperty, opened);
        FMODUWE.PlayOneShot(opened ? openSound : closeSound, soundOrigin.position);
    }

    private void TurnForceFieldOn(int targetSquareForceField)
    {
        string invokeTarget;
        
        if (targetSquareForceField == 1)
        {
            targetForcefield = squareForceField;
            targetColorControl = squareColorControl;

            invokeTarget = "DisableSquareForceField";
        }
        else
        {
            targetForcefield = customForceField;
            targetColorControl = customColorControl;

            invokeTarget = "DisableCustomForceField";
        }
        
        CancelInvoke(invokeTarget);
        
        targetColorControl.gameObject.SetActive(true);
        targetColorControl.ResetColor();
        targetColorControl.reverse = true;

        targetForcefield.SetActive(true);

        targetColorControl.Play();
    }

    private void TurnForceFieldOff(int targetSquareForceField)
    {
        string invokeTarget;
        
        if (targetSquareForceField == 1)
        {
            targetForcefield = squareForceField;
            targetColorControl = squareColorControl;

            invokeTarget = "DisableSquareForceField";
        }
        else
        {
            targetForcefield = customForceField;
            targetColorControl = customColorControl;

            invokeTarget = "DisableCustomForceField";
        }
        
        targetColorControl.ResetColor();
        targetColorControl.reverse = false;
        targetColorControl.Play();
        
        Invoke(invokeTarget, targetColorControl.duration);
    }
    
    private void DisableCustomForceField()
    {
        customColorControl.gameObject.SetActive(false);
    }

    private void DisableSquareForceField()
    {
        squareColorControl.gameObject.SetActive(false);
    }

    private void InitSparkFX()
    {
        fxSparksInstances = new GameObject[4];
        
        for (int i = 0; i < fxSparksInstances.Length; i++)
        {
            var sparkInstance = Utils.SpawnZeroedAt(fxSparksPrefab, transform);

            sparkInstance.SetActive(true);
            sparkInstance.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
            
            fxSparksInstances[i] = sparkInstance;
        }
    }

    private Vector3 GetOffsetSparkPos(int sparkIndex, Transform center)
    {
        Vector3 returnPos = center.position;
        
        switch (sparkIndex)
        {
            case 0:
                returnPos = new Vector3(center.position.x + sparkFXOffset, center.position.y, center.position.z - sparkFXOffset);
                break;
            case 1:
                returnPos = new Vector3(center.position.x + sparkFXOffset, center.position.y, center.position.z + sparkFXOffset);
                break;
            case 2:
                returnPos = new Vector3(center.position.x - sparkFXOffset, center.position.y, center.position.z - sparkFXOffset);
                break;
            case 3:
                returnPos = new Vector3(center.position.x - sparkFXOffset, center.position.y, center.position.z + sparkFXOffset);
                break;
            default:
                Plugin.Logger.LogError("Invalid spark index given for Precursor Fabricator!");
                break;
        }

        return returnPos;
    }
    
}