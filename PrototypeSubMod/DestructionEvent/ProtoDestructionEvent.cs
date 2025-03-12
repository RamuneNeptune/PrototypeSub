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

        if (Player.main.currentSub == subRoot)
        {
            internalSequence.StartSequence(subRoot);
        }
        else
        {
            externalSequence.StartSequence(subRoot);
        }
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (mixin.health > 0) return;

        StartCoroutine(OnDestroySub());
    }
}
