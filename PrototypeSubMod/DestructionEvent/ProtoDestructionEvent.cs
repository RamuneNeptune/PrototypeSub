using SubLibrary.SubFire;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.DestructionEvent;

internal class ProtoDestructionEvent : MonoBehaviour, IOnTakeDamage
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private LiveMixin mixin;

    [Header("Sequences")]
    [SerializeField] private DestructionSequence internalSequence;
    [SerializeField] private DestructionSequence externalSequence;

    private void Start()
    {
        DevConsole.RegisterConsoleCommand(this, "destroyproto");
    }

    public IEnumerator OnDestroySub()
    {
        yield return new WaitForSeconds(18f);

        DestroySub();
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (mixin.health > 0) return;

        if (Plugin.GlobalSaveData.prototypeDestroyed) return;

        StartCoroutine(OnDestroySub());
    }

    private void DestroySub()
    {
        Plugin.GlobalSaveData.prototypeDestroyed = true;

        CleanupSub();
        StartSequences();
        
        subRoot.GetComponent<ProtoSaveStateManager>().UpdateManagerStatus();
    }

    public void DestroySubNoSequence()
    {
        Plugin.GlobalSaveData.prototypeDestroyed = true;
        subRoot.GetComponent<ProtoSaveStateManager>().UpdateManagerStatus();
    }

    private void CleanupSub()
    {
        subRoot.subWarning = false;
        subRoot.fireSuppressionState = false;
        subRoot.silentRunning = false;
        subRoot.GetComponentInChildren<SubFloodAlarm>().NewAlarmState();
        foreach (var item in subRoot.GetComponentsInChildren<FMOD_CustomEmitter>(true))
        {
            item.Stop();
        }

        foreach (var damagePoint in subRoot.GetComponentsInChildren<CyclopsDamagePoint>(true))
        {
            damagePoint.OnRepair();
        }

        foreach (var room in subRoot.GetComponentsInChildren<SubRoom>(true))
        {
            var nodes = room.GetSpawnNodes();
            foreach (var node in nodes)
            {
                for (int i = 0; i < node.childCount; i++)
                {
                    Destroy(node.GetChild(i).gameObject);
                }
            }
        }
    }
    
    private void StartSequences()
    {
        if (Player.main.currentSub == subRoot)
        {
            internalSequence.StartSequence(subRoot);
        }
        else
        {
            externalSequence.StartSequence(subRoot);
        }

    }

    private void OnConsoleCommand_destroyproto(NotificationCenter.Notification n)
    {
        DestroySub();
    }
}
