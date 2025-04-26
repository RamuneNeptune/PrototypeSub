using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal.Relays;

public class RelayInstallationButton : MonoBehaviour
{
    [SerializeField] private Image[] images;

    public void SetColor(float fillCol)
    {
        foreach (var image in images)
        {
            image.color = new Color(fillCol, fillCol,fillCol);
        }
    }
}