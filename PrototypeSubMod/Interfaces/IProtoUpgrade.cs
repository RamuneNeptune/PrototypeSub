namespace PrototypeSubMod.Interfaces;

internal interface IProtoUpgrade
{
    public void SetUpgradeActive(bool active);
    public bool GetUpgradeActive();
    public string GetUpgradeName();
}
