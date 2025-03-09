using UnityEngine;

namespace PrototypeSubMod.Facilities;

internal class AddIdentifierOnPickup : MonoBehaviour
{
    private Pickupable pickupable;
    private string classID;
    private string id;

    private void Start()
    {
        pickupable = GetComponent<Pickupable>();
        pickupable.pickedUpEvent.AddHandler(this, (pickupable) => OnPickup());
    }

    private void OnPickup()
    {
        var identifier = gameObject.AddComponent<PrefabIdentifier>();
        identifier.ClassId = classID;
        identifier.Id = id;
    }

    public void SetOriginalValues(string classID, string id)
    {
        this.classID = classID;
        this.id = id;
    }
}
