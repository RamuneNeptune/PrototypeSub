using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

public class AmpeelAggroClearer : MonoBehaviour, IOnTakeDamage
{
    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (!damageInfo.dealer) return;

        var shocker = damageInfo.dealer.GetComponent<Shocker>();

        if (!shocker) return;

        shocker.liveMixin.TakeDamage(10);
    }
}