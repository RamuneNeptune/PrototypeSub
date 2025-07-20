using System;
using BepInEx.Bootstrap;
using PrototypeSubMod.Patches;
using UnityEngine;

namespace PrototypeSubMod.Compatibility;

public class VehicleFrameworkCompatManager
{
    public static void TryEndDocking(Vehicle vehicle)
    {
        if (!IsModdedVehicle(vehicle)) return;
        
        VehicleFrameworkCompat_Patches.OnDockedMethod.Invoke(vehicle, new object[] { Vector3.zero });
        VehicleFrameworkCompat_Patches.PlayerExitMethod.Invoke(vehicle, null);
    }

    public static void OnVehicleUndocked(Vehicle vehicle)
    {
        if (!IsModdedVehicle(vehicle)) return;
        
        VehicleFrameworkCompat_Patches.OnUndockedMethod.Invoke(vehicle, null);
    }

    public static bool IsModdedVehicle(Vehicle vehicle)
    {
        if (!VehicleFrameworkInstalled()) return false;
        
        return vehicle.GetType().IsSubclassOf(VehicleFrameworkCompat_Patches.ModVehicleType);
    }

    public static bool VehicleFrameworkInstalled() =>
        Chainloader.PluginInfos.ContainsKey("com.mikjaw.subnautica.vehicleframework.mod");
}