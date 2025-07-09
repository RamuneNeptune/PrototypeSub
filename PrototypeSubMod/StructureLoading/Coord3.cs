using System;
using Newtonsoft.Json;
using UnityEngine;

namespace PrototypeSubMod.StructureLoading;

[Serializable]
public struct Coord3
{
    [JsonConstructor]
    public Coord3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Coord3(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }

    public readonly Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public readonly float x;

    public readonly float y;

    public readonly float z;
}