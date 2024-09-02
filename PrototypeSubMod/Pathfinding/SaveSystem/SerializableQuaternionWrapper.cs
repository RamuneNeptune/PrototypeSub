using System;
using UnityEngine;

namespace PrototypeSubMod.Pathfinding.SaveSystem;

[Serializable]
public class SerializableQuaternionWrapper
{
    public float x;
    public float y;
    public float z;
    public float w;

    public Quaternion Quaternion
    {
        get
        {
            return new Quaternion(x, y, z, w);
        }
    }

    public SerializableQuaternionWrapper(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public SerializableQuaternionWrapper(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }
}
