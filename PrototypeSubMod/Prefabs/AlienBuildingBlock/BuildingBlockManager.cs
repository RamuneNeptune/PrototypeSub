﻿using UnityEngine;

namespace PrototypeSubMod.Prefabs.AlienBuildingBlock;

public class BuildingBlockManager : MonoBehaviour
{

    public string spawnBiome;
    public bool warperBlock;

    [SerializeField]
    private float glowStrengthChangeRate = 0.005f;

    [SerializeField]
    private float maxGlowStrength = 13f;

    private float glowStrengthNight;

    private Material blockMaterial;

    private bool blockActive;
    private bool increaseGlow;

    private void Awake()
    {
        if (LargeWorld.main)
            spawnBiome = LargeWorld.main.GetBiome(transform.position);

        blockMaterial = gameObject.GetComponentInChildren<MeshRenderer>().materials[0];

        blockActive = gameObject.GetComponent<TechTag>().type == AlienBuildingBlock.prefabInfo.TechType;

        if (!blockActive)
            glowStrengthNight = 0f;
    }

    private void Update()
    {
        if (!blockActive)
        {
            if (glowStrengthNight <= 0)
                increaseGlow = true;
            else if (glowStrengthNight >= maxGlowStrength)
                increaseGlow = false;

            glowStrengthNight += increaseGlow ? glowStrengthChangeRate : -glowStrengthChangeRate;
            
            blockMaterial.SetFloat(ShaderPropertyID._GlowStrengthNight, glowStrengthNight);
        }
    }
}