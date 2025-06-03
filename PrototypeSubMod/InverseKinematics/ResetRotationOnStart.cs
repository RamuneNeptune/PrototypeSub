using System;
using UnityEngine;

namespace DitzelGames.FastIK;

public class ResetRotationOnStart : MonoBehaviour
{
    private Quaternion rotation;

    private void Awake()
    {
        rotation = transform.rotation;
        transform.rotation = Quaternion.identity;
    }

    private void Start()
    {
        transform.rotation = rotation;
    }
}