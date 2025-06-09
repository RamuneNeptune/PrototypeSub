using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class PlayInterfloorFX : MonoBehaviour
{
    [SerializeField] private FMODAsset soundEffect;
    [SerializeField] private float duration;
    
    public void PlayEffect()
    {
        FMODUWE.PlayOneShot(soundEffect, transform.position, 0.25f);
        InterfloorTeleporter.PlayTeleportEffect(duration);
    }
}