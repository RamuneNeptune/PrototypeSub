using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

public class BiomeSkyApplier : MonoBehaviour
{

    public SkyApplier skyApplier;

    public String mimicBiome;

    private string currentBiome;

    private static List<Tuple<String, String>> biomeSkies = new()
    {
        Tuple.Create("mountains", "14cd4cc9-93ef-4104-ae95-f8cee52a5698"),
        Tuple.Create("LostRiver_Junction", "80f6c46a-ecfe-4a19-b05f-0466eafde411"),
        Tuple.Create("ILZChamber", "a84f22af-9802-49c2-92ff-5c58335593a1"),
        Tuple.Create("LavaLakes", "e8143977-448e-4202-b780-83485fa5f31a")
    };

    private void Awake()
    {
        if (!mimicBiome.IsNullOrWhiteSpace())
        {
            string mimicClassID = null;

            for (int i = 0; i < biomeSkies.Count; i++)
            {
                if (mimicBiome.Equals(biomeSkies[i].Item1))
                {
                    mimicClassID = biomeSkies[i].Item2;
                    break;
                }
            }

            if (mimicClassID.IsNullOrWhiteSpace())
            {
                Plugin.Logger.LogError("Invalid BiomeMimic string given for BiomeSkyApplier!");
                return;
            }

            CoroutineHost.StartCoroutine(SetCustomSky(mimicClassID));
            return;
        }
        
        if (LargeWorld.main)
            currentBiome = LargeWorld.main.GetBiome(transform.position);

        if (!currentBiome.IsNullOrWhiteSpace())
        {
            for (int i = 0; i < biomeSkies.Count; i++)
            {
                if (currentBiome.Equals(biomeSkies[i].Item1))
                {
                    CoroutineHost.StartCoroutine(SetCustomSky(biomeSkies[i].Item2));
                    return;
                }
            }
            
            skyApplier.anchorSky = Skies.Auto;
        }
    }

    private IEnumerator SetCustomSky(string classID)
    {
        if (skyApplier.customSkyPrefab != null)
            yield break;
        
        var vanillaPrefabTask = PrefabDatabase.GetPrefabAsync(classID);
        
        yield return vanillaPrefabTask;

        vanillaPrefabTask.TryGetPrefab(out var vanillaPrefab);

        skyApplier.customSkyPrefab = vanillaPrefab.GetComponent<SkyApplier>().customSkyPrefab;
    }
}