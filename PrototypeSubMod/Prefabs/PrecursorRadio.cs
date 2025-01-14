﻿using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using PrototypeSubMod.Utility;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Prefabs;

internal class PrecursorRadio
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("PrecursorRadio", null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);

        prefab.SetGameObject(GetPrefab);
        prefab.SetSpawns(new[]
        {
            new SpawnLocation(new Vector3(402.734f, -76.59f, 1052.08f))
        });

        prefab.Register();
    }

    private static IEnumerator GetPrefab(IOut<GameObject> prefabOut)
    {
        var prefab = Plugin.AssetBundle.LoadAsset<GameObject>("PrecursorRadio");
        prefab.SetActive(false);

        var instance = GameObject.Instantiate(prefab);

        yield return new WaitUntil(() => MaterialUtils.IsReady);

        MaterialUtils.ApplySNShaders(instance, modifiers: new ProtoMaterialModifier(3, 0));

        prefabOut.Set(instance);
    }
}
