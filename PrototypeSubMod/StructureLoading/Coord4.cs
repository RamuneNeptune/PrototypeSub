using System;
using Newtonsoft.Json;
using UnityEngine;

namespace PrototypeSubMod.StructureLoading;

[Serializable]
public struct Coord4
{
    [JsonConstructor]
    public Coord4(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Coord4(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }

    public readonly Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }

    public readonly float x;

    public readonly float y;

    public readonly float z;

    public readonly float w;
}