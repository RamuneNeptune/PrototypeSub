namespace PrototypeSubMod.Interfaces;

internal interface IProtoUpgrade
{
    public void SetUpgradeInstalled(bool installed);
    public bool GetUpgradeInstalled();
    public void SetUpgradeEnabled(bool enabled);
    public bool GetUpgradeEnabled();
    public string GetUpgradeName();
}
