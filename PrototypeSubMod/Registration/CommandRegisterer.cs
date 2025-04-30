using Nautilus.Handlers;
using UnityEngine;

namespace PrototypeSubMod.Registration;

internal static class CommandRegisterer
{
    public static void Register()
    {
        ConsoleCommandsHandler.AddGotoTeleportPosition("interceptorfacility", new Vector3(547, -709, 955));
        ConsoleCommandsHandler.AddGotoTeleportPosition("defensefacility", new Vector3(689, -483, -1404f));
        ConsoleCommandsHandler.AddGotoTeleportPosition("enginefacility", new Vector3(306, -1156, 131f));
        ConsoleCommandsHandler.AddGotoTeleportPosition("hullfacility", new Vector3(-1260, -614, -352f));
        ConsoleCommandsHandler.AddGotoTeleportPosition("ppt", new Vector3(449, -92, 1169));
    }
}
