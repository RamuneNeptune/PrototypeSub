using System;
using System.Reflection;

namespace PrototypeSubMod.Compatibility;

internal class WeatherCompatManager
{
    public static bool WeatherInstalled
    {
        get
        {
            if (!_weatherInitted)
            {
                _weatherInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.lee23.epicweather");
                _weatherInitted = true;
            }

            return _weatherInstalled;
        }
    }

    private static bool _weatherInitted;
    private static bool _weatherInstalled;

    private static bool _initialized;

    private static MethodInfo setWeatherMethod;
    private static MethodInfo setEnabledMethod;
    private static FieldInfo mainManager;
    private static Type clearSkiesType;

    public static void Initialize()
    {
        if (!WeatherInstalled || _initialized) return;

        Type customWeatherManager = Type.GetType("WeatherMod.Mono.CustomWeatherManager, WeatherMod, Version=0.0.5.0, Culture=neutral, PublicKeyToken=null");
        setWeatherMethod = customWeatherManager.GetMethod("SetWeather", BindingFlags.Public | BindingFlags.Instance);
        setEnabledMethod = customWeatherManager.GetMethod("set_enabled", BindingFlags.Public | BindingFlags.Instance);

        clearSkiesType = Type.GetType("WeatherMod.WeatherEvents.ClearSkies, WeatherMod, Version=0.0.5.0, Culture=neutral, PublicKeyToken=null");
        mainManager = customWeatherManager.GetField("Main", BindingFlags.Static | BindingFlags.Public);

        _initialized = true;
    }

    public static void SetWeatherClear()
    {
        if (!WeatherInstalled) return;

        object instance = Activator.CreateInstance(clearSkiesType);
        object main = mainManager.GetValue(null);
        setWeatherMethod.Invoke(main, new[] { instance });
    }

    public static void SetWeatherEnabled(bool enabled)
    {
        if (!WeatherInstalled) return;

        object main = mainManager.GetValue(null);
        setEnabledMethod.Invoke(main, new object[] { enabled });
    }
}
