using System;
using UnityEngine;

namespace PrototypeSubMod.Facilities;

public abstract class InteractableTerminal : MonoBehaviour
{
    public abstract event Action onTerminalInteracted;

    public abstract void ForceInteracted();
}