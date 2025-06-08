using System.Collections;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using UnityEngine;

namespace PrototypeSubMod.Docking;

public class ProtoDockingManager : MonoBehaviour, IAnimParamReceiver, IProtoEventListener, IProtoTreeEventListener
{
    [SerializeField] private InterfloorTeleporter interfloorTeleporter;
    [SerializeField] private VehicleDockingBay dockingBay;
    [SerializeField] private IgnoreCinematicStart ignoreCinematicStart;
    [SerializeField] private Transform playerPosition;
    [SerializeField] private Transform vehicleHolder;

    private bool cinematicStartManaged;
    
    private void Start()
    {
        if (cinematicStartManaged) return;
        
        ignoreCinematicStart.enabled = false;
    }

    private IEnumerator InitializeVehicle()
    {
        if (vehicleHolder.childCount == 0) yield break;

        cinematicStartManaged = true;
        ignoreCinematicStart.enabled = true;

        yield return new WaitForEndOfFrame();
        
        var vehicle = vehicleHolder.GetChild(0).gameObject;
        vehicle.SetActive(true);
        
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => dockingBay._dockedVehicle);
        
        ForwardAnimationParameterBool(null, false);
        ignoreCinematicStart.enabled = false;
        cinematicStartManaged = false;
    }
    
    public void TeleportIntoSub()
    {
        playerPosition.position = Camera.main.transform.position;
        interfloorTeleporter.StartTeleportPlayer();
    }

    public void ForwardAnimationParameterBool(string paramaterName, bool value)
    {
        if (value) return;
        
        if (!dockingBay.dockedVehicle) return;
        
        // Cinematic mode finished
        dockingBay.dockedVehicle.transform.SetParent(vehicleHolder);
        dockingBay.dockedVehicle.gameObject.SetActive(false);
    }

    public void Undock()
    {
        UWE.CoroutineHost.StartCoroutine(UndockDelayed());
    }

    private IEnumerator UndockDelayed()
    {
        FMODUWE.PlayOneShot(interfloorTeleporter.GetFMODAsset(), transform.position, 0.25f);
        InterfloorTeleporter.PlayTeleportEffect(0.2f);

        yield return new WaitForSeconds(0.1f);
        
        vehicleHolder.GetChild(0).gameObject.SetActive(true);
        dockingBay.OnUndockingComplete(Player.main);
    }

    public void OnProtoSerializeObjectTree(ProtobufSerializer serializer) { }

    public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
    {
        UWE.CoroutineHost.StartCoroutine(InitializeVehicle());
    }

    public void OnProtoSerialize(ProtobufSerializer serializer) { }

    public void OnProtoDeserialize(ProtobufSerializer serializer)
    {
        ignoreCinematicStart.enabled = true;
    }
}