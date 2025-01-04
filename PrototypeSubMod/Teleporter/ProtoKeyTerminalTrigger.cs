namespace PrototypeSubMod.Teleporter;

internal class ProtoKeyTerminalTrigger : PrecursorKeyTerminalTrigger
{
    private bool locked;

    public bool GetIsLocked() => locked;
    public void SetStoryLocked(bool locked)
    {
        this.locked = locked;
    }

    public void OnClickDenied()
    {
        ErrorMessage.AddError("Insert interceptor denied voiceline here");
    }
}
