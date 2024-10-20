using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

[RequireComponent(typeof(Collider))]
internal class PowerAbilitySystemProxy : MonoBehaviour
{
    [SerializeField] private ProtoPowerAbilitySystem abilitySystem;

    private void OnTriggerEnter(Collider col)
    {
        if (!col.gameObject.Equals(Player.main.gameObject)) return;

        abilitySystem.OnEnterProxy();
    }

    private void OnTriggerExit(Collider col)
    {
        if (!col.gameObject.Equals(Player.main.gameObject)) return;

        abilitySystem.OnExitProxy();
    }
}
