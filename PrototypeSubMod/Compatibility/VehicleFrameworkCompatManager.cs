using System;
using BepInEx.Bootstrap;
using PrototypeSubMod.Patches;
using UnityEngine;

namespace PrototypeSubMod.Compatibility;

public class VehicleFrameworkCompatManager
{
    public static void TryEndDocking(Vehicle vehicle)
    {
        if (!Chainloader.PluginInfos.ContainsKey("com.mikjaw.subnautica.vehicleframework.mod")) return;

        if (!vehicle.GetType().IsAssignableFrom(VehicleFrameworkCompat_Patches.ModVehicleType)) return;
        
        VehicleFrameworkCompat_Patches.OnDockedMethod.Invoke(vehicle, new object[] { Vector3.zero });
        VehicleFrameworkCompat_Patches.PlayerExitMethod.Invoke(vehicle, null);
    }

    public static void OnVehicleUndocked(Vehicle vehicle)
    {
        if (!Chainloader.PluginInfos.ContainsKey("com.mikjaw.subnautica.vehicleframework.mod")) return;

        Plugin.Logger.LogInfo($"Vehicle type = {vehicle.GetType()} | Assignable = {vehicle.GetType().IsAssignableFrom(VehicleFrameworkCompat_Patches.ModVehicleType)}");
        
        if (!vehicle.GetType().IsSubclassOf(VehicleFrameworkCompat_Patches.ModVehicleType)) return;
        
        VehicleFrameworkCompat_Patches.OnUndockedMethod.Invoke(vehicle, null);
    }
}