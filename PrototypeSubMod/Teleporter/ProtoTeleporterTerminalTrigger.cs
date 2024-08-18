using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class ProtoTeleporterTerminalTrigger : MonoBehaviour
{
    private SubRoot subRoot;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject != Player.main.gameObject) return;

        if (Player.main.currentSub != subRoot) return;

        SendMessageUpwards("OpenDeck");
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject != Player.main.gameObject) return;

        SendMessageUpwards("CloseDeck");
    }

    public void SetSubRoot(SubRoot subRoot)
    {
        this.subRoot = subRoot;
    }
}
