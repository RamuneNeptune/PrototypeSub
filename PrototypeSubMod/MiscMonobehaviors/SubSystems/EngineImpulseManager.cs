using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class EngineImpulseManager : MonoBehaviour
{
    [SerializeField] private EngineRpmSFXManager rpmManager;
    [SerializeField] private FMOD_CustomEmitter impulseSfx;
    [SerializeField] private float timeToSfxAllowed;

    private float timeSfxPlayed;
    private float currentSfxTime;
    private float prevRpmSpeed;
    private bool sfxPlayed;
    
    // Called via SubRoot.BroadcastMessage
    // Happens when the player exits the sub
    public void SaveEngineStateAndPowerDown()
    {
        if (currentSfxTime > 0 || sfxPlayed)
        {
            currentSfxTime = timeToSfxAllowed;
            sfxPlayed = false;
        }
    }

    private void Update()
    {
        if (currentSfxTime > 0)
        {
            currentSfxTime -= Time.deltaTime;
        }

        if ((rpmManager.rpmSpeed > 0 && prevRpmSpeed == 0 && !sfxPlayed) || (Time.time < timeSfxPlayed + 0.5f && !impulseSfx.playing))
        {
            sfxPlayed = true;
            impulseSfx.Play();
            timeSfxPlayed = Time.time;
        }
        else if (rpmManager.rpmSpeed == 0 && prevRpmSpeed > 0)
        {
            impulseSfx.Stop();
        }
        
        prevRpmSpeed = rpmManager.rpmSpeed;
    }
}