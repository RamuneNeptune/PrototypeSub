namespace PrototypeSubMod.PowerSystem.Funcionalities;

internal class IonCubePowerFunctionality : PowerSourceFunctionality
{
    public override void OnCountChanged(bool added)
    {
        base.OnActivated(added);
        ErrorMessage.AddMessage($"Ion cube added = {added}");
    }
}
