using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

internal class ProtoUpgradeScreenManager : MonoBehaviour
{
    [SerializeField] private UpgradeScreen defaultUpgradeScreen;

    private UpgradeScreen currentScreen;

    private void Start()
    {
        currentScreen = defaultUpgradeScreen;
    }

    public void SetCurrentScreen(UpgradeScreen screen)
    {
        currentScreen.SetInteractable(false);
        currentScreen.SetTargetAlpha(0f);

        currentScreen = screen;

        currentScreen.SetInteractable(true);
        currentScreen.SetTargetAlpha(1f);
    }
}
