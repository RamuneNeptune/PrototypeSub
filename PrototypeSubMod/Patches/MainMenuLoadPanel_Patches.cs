using HarmonyLib;
using PrototypeSubMod.SaveData;
using PrototypeSubMod.SaveIcon;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(MainMenuLoadPanel))]
internal class MainMenuLoadPanel_Patches
{
    [HarmonyPatch(nameof(MainMenuLoadPanel.UpdateLoadButtonState)), HarmonyPostfix]
    private static void UpdateLoadButtonState_Postfix(MainMenuLoadButton lb)
    {
        var protoIcon = lb.saveIcons.FindChild("SavedPrototype");

        if (protoIcon == null)
        {
            protoIcon = GameObject.Instantiate(lb.saveIcons.FindChild("SavedCyclops").gameObject, lb.saveIcons.transform);

            protoIcon.name = "SavedPrototype";

            protoIcon.GetComponent<Image>().sprite = Plugin.TitleAssetBundle.LoadAsset<Sprite>("ProtoSaveIcon");
            protoIcon.SetActive(false);
        }

        SaveSlotManager.SaveSlotContainsData(lb.saveGame, "PrototypeSubMod\\PrototypeSubMod.json", (bool success, ProtoGlobalSaveData saveData) =>
        {
            if (!success) return;

            protoIcon.SetActive(saveData.prototypePresent);
        });
    }
}
