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
            protoIcon = GameObject.Instantiate(lb.saveIcons.FindChild("SavedRocket").gameObject, lb.saveIcons.transform);

            protoIcon.name = "SavedPrototype";

            protoIcon.GetComponent<Image>().sprite = Plugin.PrototypeSaveIcon;
            protoIcon.SetActive(false);
        }

        if (SaveSlotManager.SaveContainsProtoData<ProtoGlobalSaveData>(lb.saveGame, out var saveData))
        {
            protoIcon.SetActive(saveData.prototypePresent);
        }
    }
}
