using Nautilus.Handlers;
using UnityEngine;

namespace PrototypeSubMod.Registration;

internal class CommandRegisterer
{
    public static void Register()
    {
        ConsoleCommandsHandler.AddGotoTeleportPosition("interceptorfacility", new Vector3(547, -709, 955));
        ConsoleCommandsHandler.AddGotoTeleportPosition("defensefacility", new Vector3(689, -483, -1404f));
        ConsoleCommandsHandler.AddGotoTeleportPosition("enginefacility", new Vector3(306, -1156, 131f));
    }
}
