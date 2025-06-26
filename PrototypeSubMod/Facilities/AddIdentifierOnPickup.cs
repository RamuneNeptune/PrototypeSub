using UnityEngine;

namespace PrototypeSubMod.Facilities;

internal class AddIdentifierOnPickup : MonoBehaviour
{
    private Pickupable pickupable;
    private LargeWorldEntity.CellLevel? cellLevel;
    private string classID;
    private string id;

    private void Start()
    {
        pickupable = GetComponent<Pickupable>();
        pickupable.pickedUpEvent.AddHandler(this, OnPickup);
    }

    private void OnPickup(Pickupable pickup)
    {
        var identifier = gameObject.EnsureComponent<PrefabIdentifier>();
        identifier.ClassId = classID;
        identifier.Id = id;

        if (cellLevel != null)
        {
            var lwe = gameObject.EnsureComponent<LargeWorldEntity>();
            lwe.cellLevel = cellLevel.Value;
        }
        
        pickupable.pickedUpEvent.RemoveHandler(this, OnPickup);
        Destroy(this);
    }

    public void SetOriginalValues(string classID, string id, LargeWorldEntity.CellLevel? cellLevel)
    {
        this.classID = classID;
        this.id = id;
        this.cellLevel = cellLevel;
    }
}
