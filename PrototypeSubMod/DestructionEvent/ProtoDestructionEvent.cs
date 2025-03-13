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

    public IEnumerator OnDestroySub()
    {
        yield return new WaitForSeconds(18f);

        ErrorMessage.AddError($"The sub just got destroyed!");
        subRoot.subWarning = false;
        subRoot.fireSuppressionState = false;
        subRoot.silentRunning = false;
        subRoot.GetComponentInChildren<SubFloodAlarm>().NewAlarmState();
        foreach (var item in subRoot.GetComponentsInChildren<FMOD_CustomEmitter>(true))
        {
            item.Stop();
        }

        foreach (var item in subRoot.GetComponentsInChildren<Fire>(true))
        {
            item.Douse(200f);
        }

        if (Player.main.currentSub == subRoot)
        {
            internalSequence.StartSequence(subRoot);
        }
        else
        {
            externalSequence.StartSequence(subRoot);
        }

        Plugin.GlobalSaveData.prototypeDestroyed = true;
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (mixin.health > 0) return;

        StartCoroutine(OnDestroySub());
    }
}
