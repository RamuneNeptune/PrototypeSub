using PrototypeSubMod.Pathfinding.SaveSystem;

namespace PrototypeSubMod.Pathfinding;

[System.Serializable]
public class GridNode : IHeapItem<GridNode>
{
    public bool walkable;
    public SerializableV3Wrapper worldPosition;
    public SerializableV3Wrapper pointOnBounds;
    public SerializableV3Wrapper surfaceNormal;
    public int gridPosX;
    public int gridPosY;
    public int gridPosZ;
    public int movementPenalty;

    [SaveIgnore] public int gCost; // How far the node is from the starting node
    [SaveIgnore] public int hCost; // How far the node is from the end node
    [SaveIgnore] public GridNode parent;
    
    public GridNode() { }

    public GridNode(bool walkable, SerializableV3Wrapper worldPosition, SerializableV3Wrapper pointOnBounds, SerializableV3Wrapper surfaceNormal, int gridPosX, int gridPosY, int gridPosZ, int penalty)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.pointOnBounds = pointOnBounds;
        this.surfaceNormal = surfaceNormal;
        this.gridPosX = gridPosX;
        this.gridPosY = gridPosY;
        this.gridPosZ = gridPosZ;
        movementPenalty = penalty;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return _heapIndex;
        }
        set
        {
            _heapIndex = value;
        }
    }

    private int _heapIndex;

    public int CompareTo(GridNode other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }

        return -compare;
    }
}
