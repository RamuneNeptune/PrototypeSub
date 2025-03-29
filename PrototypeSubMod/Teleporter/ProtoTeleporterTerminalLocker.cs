using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class ProtoTeleporterTerminalLocker : PrecursorKeyTerminalTrigger
{
    [SerializeField] private PrecursorTeleporter teleporter;
    [SerializeField] private VoiceNotificationManager voiceNotificationManager;
    [SerializeField] private VoiceNotification interceptorLockedNotif;
    
    private bool locked;

    public bool GetIsLocked() => locked;
    public void SetStoryLocked(bool locked)
    {
        this.locked = locked;
        if (locked)
        {
            teleporter.ToggleDoor(false);
        }
    }

    public void OnClickDenied()
    {
        voiceNotificationManager.PlayVoiceNotification(interceptorLockedNotif);
    }
}
