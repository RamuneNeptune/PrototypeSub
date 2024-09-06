using System;
using UnityEngine;

namespace PrototypeSubMod.Pathfinding.SaveSystem;

[Serializable]
public class SerializableV3Wrapper
{
    public float x;
    public float y;
    public float z;

    public Vector3 Vector
    {
        get
        {
            return new Vector3(x, y, z);
        }
    }

    public SerializableV3Wrapper(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public SerializableV3Wrapper(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override string ToString()
    {
        return Vector.ToString();
    }
}
