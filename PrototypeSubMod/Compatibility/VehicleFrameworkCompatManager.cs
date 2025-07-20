using BepInEx.Bootstrap;
using PrototypeSubMod.Patches;
using UnityEngine;

namespace PrototypeSubMod.Compatibility;

public class VehicleFrameworkCompatManager
{
    public static void TryEndDocking(Vehicle vehicle)
    {
        if (!Chainloader.PluginInfos.ContainsKey("com.mikjaw.subnautica.vehicleframework.mod")) return;
        
        VehicleFrameworkCompat_Patches.OnDockedMethod.Invoke(vehicle, new object[] { Vector3.zero });
        VehicleFrameworkCompat_Patches.PlayerExitMethod.Invoke(vehicle, null);
    }
}