namespace PrototypeSubMod.Pathfinding.SaveSystem;

[System.Serializable]
public class GridSaveData
{
    public GridNode[,,] grid;
    public SerializableV3Wrapper positionAtGridGen;
    public SerializableQuaternionWrapper rotationAtGridGen;

    public GridSaveData(GridNode[,,] grid, SerializableV3Wrapper positionAtGridGen, SerializableQuaternionWrapper rotationAtGridGen)
    {
        this.grid = grid;
        this.positionAtGridGen = positionAtGridGen;
        this.rotationAtGridGen = rotationAtGridGen;
    }
}
