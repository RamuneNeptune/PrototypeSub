using TMPro;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

public class SetAllTextDirty : MonoBehaviour
{
    private TextMeshProUGUI[] texts;
    
    private void Start()
    {
        texts = GetComponentsInChildren<TextMeshProUGUI>(true);
    }

    public void SetDirty()
    {
        foreach (var text in texts)
        {
            text.SetAllDirty();
        }
    }
}