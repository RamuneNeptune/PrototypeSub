using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.DestructionEvent;

internal class ProtoDestructionEvent : MonoBehaviour, IOnTakeDamage
{
    [SerializeField] private LiveMixin mixin;

    public IEnumerator OnDestroySub()
    {
        yield return new WaitForSeconds(18f);

        ErrorMessage.AddError($"The sub just got destroyed!");
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (mixin.health > 0) return;

        StartCoroutine(OnDestroySub());
    }
}
