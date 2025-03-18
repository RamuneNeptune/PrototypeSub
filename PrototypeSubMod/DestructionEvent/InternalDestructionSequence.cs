using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Patches;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.DestructionEvent;

internal class InternalDestructionSequence : DestructionSequence
{
    private static readonly Vector3 VoidTeleportPos = new Vector3(-2030, -612, -1551);

    [SerializeField] private Transform playerPos;
    [SerializeField] private InterfloorTeleporter[] teleporters;

    private void Start()
    {
        Player.main.playerDeathEvent.AddHandler(this, OnPlayerDeath);
    }

    public override void StartSequence(SubRoot subRoot)
    {
        UWE.CoroutineHost.StartCoroutine(TeleportToVoid(subRoot));

        foreach (var teleporter in teleporters)
        {
            teleporter.enabled = false;
        }
    }

    private IEnumerator TeleportToVoid(SubRoot subRoot)
    {
        IngameMenu_Patches.SetDenySaving(true);
        InterfloorTeleporter.PlayTeleportEffect(3f);
        subRoot.GetComponent<PingInstance>().enabled = false;
        subRoot.GetComponent<VoiceNotificationManager>().enabled = false;
        yield return new WaitForSeconds(0.5f);

        Player.main.SetCurrentSub(null, true);
        Player.main.SetPosition(VoidTeleportPos + subRoot.transform.InverseTransformPoint(playerPos.position));
        subRoot.transform.position = VoidTeleportPos;
    }

    private void OnPlayerDeath(Player player)
    {
        IngameMenu_Patches.SetDenySaving(false);
    }
}
