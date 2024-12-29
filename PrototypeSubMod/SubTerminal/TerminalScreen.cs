using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

internal abstract class TerminalScreen : MonoBehaviour
{
    public abstract void OnStageStarted();
    public abstract void OnStageFinished();
}
