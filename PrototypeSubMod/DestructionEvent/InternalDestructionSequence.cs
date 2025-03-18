using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Patches;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.DestructionEvent;

internal class InternalDestructionSequence : DestructionSequence
{
    private static readonly Vector3 VoidTeleportPos = new Vector3();

    private void Start()
    {
        Player.main.playerDeathEvent.AddHandler(this, OnPlayerDeath);
    }

    public override void StartSequence(SubRoot subRoot)
    {
        UWE.CoroutineHost.StartCoroutine(TeleportToVoid(subRoot));
    }

    private IEnumerator TeleportToVoid(SubRoot subRoot)
    {
        IngameMenu_Patches.SetDenySaving(true);
        InterfloorTeleporter.PlayTeleportEffect(3f);
        yield return new WaitForSeconds(0.5f);

        Vector3 localPlayerPos = subRoot.transform.InverseTransformPoint(Player.main.transform.position);
        subRoot.transform.position = VoidTeleportPos;
        Player.main.SetPosition(VoidTeleportPos + localPlayerPos);
    }

    private void OnPlayerDeath(Player player)
    {
        IngameMenu_Patches.SetDenySaving(false);
    }
}
