using System.Collections;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using UnityEngine;

namespace PrototypeSubMod.Docking;

public class ProtoDockingManager : MonoBehaviour, IProtoEventListener, IProtoTreeEventListener
{
    [SerializeField] private InterfloorTeleporter interfloorTeleporter;
    [SerializeField] private VehicleDockingBay dockingBay;
    [SerializeField] private IgnoreCinematicStart ignoreCinematicStart;
    [SerializeField] private Transform playerPosition;
    [SerializeField] private Transform vehicleHolder;

    private Vehicle lastDockedVehicle;
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
        
        StoreVehicle();
        ignoreCinematicStart.enabled = false;
        cinematicStartManaged = false;
    }
    
    public void TeleportIntoSub()
    {
        if (dockingBay.dockedVehicle == Player.main.currentMountedVehicle)
        {
            interfloorTeleporter.StartTeleportPlayer();
        }

        Invoke(nameof(StoreVehicle), 0.1f);
    }

    public void StoreVehicle()
    {
        if (!dockingBay.dockedVehicle) return;
        
        dockingBay.dockedVehicle.transform.SetParent(vehicleHolder);
        dockingBay.dockedVehicle.gameObject.SetActive(false);
    }

    public void Undock()
    {
        UWE.CoroutineHost.StartCoroutine(UndockDelayed());
    }

    // Called via VehicleDockingBay Invoke
    private void UnlockDoors()
    {
        if (!dockingBay.dockedVehicle || dockingBay.dockedVehicle == lastDockedVehicle) return;

        lastDockedVehicle = dockingBay.dockedVehicle;
        
        var localPos = dockingBay.dockedVehicle.transform.InverseTransformPoint(dockingBay.dockedVehicle.playerPosition.transform.position);
        playerPosition.localPosition = localPos;
    }
    
    private IEnumerator UndockDelayed()
    {
        FMODUWE.PlayOneShot(interfloorTeleporter.GetFMODAsset(), transform.position, 0.25f);
        InterfloorTeleporter.PlayTeleportEffect(0.2f);

        yield return new WaitForSeconds(0.1f);

        var vehicle = vehicleHolder.GetChild(0).gameObject;
        dockingBay.OnUndockingComplete(Player.main);
        vehicle.SetActive(true);
        
        var rb = vehicle.GetComponent<Rigidbody>();
        if (!rb) yield break;
        
        if (vehicle.GetComponent<Vehicle>().controlSheme == Vehicle.ControlSheme.Mech) yield break;
        
        yield return new WaitForSeconds(0.2f);
        
        rb.AddForce((rb.transform.forward - rb.transform.up) * 10f, ForceMode.VelocityChange);
        rb.AddTorque(rb.transform.right * 2, ForceMode.VelocityChange);
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