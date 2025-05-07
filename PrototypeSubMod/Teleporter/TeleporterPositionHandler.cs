using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

public static class TeleporterPositionHandler
{
    public static readonly Dictionary<string, TeleportData> TeleporterPositions = new()
    {
        { "guntpM", new(new Vector3(-27f, -1210.5f, 112.5f), 270f) },
        { "guntpS", new(new Vector3(377f, -91.5f, 1141f), 109.7f) },

        { "finaltpM", new(new Vector3(244.93f, -1589.43f, -310.38f), 151.025f) },
        { "finaltpS", new(new Vector3(457.37f, -168.5f, 1364.46f), 152.919f) },

        { "islandtpM", new(new Vector3(341f, 60f, 903f), 0f) },
        { "islandtpS", new(new Vector3(-662f, 2.00f, -1060f), 0f) },

        { "cragfieldtpM", new(new Vector3(162.36f, -1430.29f, -371.32f), 61.051f) },
        { "cragfieldtpS", new(new Vector3(-52.31f, -280f, -1232f), -120.016f) },

        { "kooshzonetpM", new(new Vector3(340.21f, -1430f, -270.5f), -118.949f) },
        { "kooshzonetpS", new(new Vector3(182.43f, -1430.29f, -409.1f), 61.051f) },

        { "lostrivertpM", new(new Vector3(182.43f, -1430.29f, -409.1f), 61.051f) },
        { "lostrivertpS", new(new Vector3(-884f, -613f, 1033f), -30f) },

        { "mushroomforesttpM", new(new Vector3(362.5f, -1430.29f, -309.28f), -118.949f) },
        { "mushroomforesttpS", new(new Vector3(-747.22f, -241.81f, 437.13f), -104.692f) },
        
        { "protoislandtpS", new (new Vector3(545.3f, 102.5f, 1740.6f), 57.7f) }
    };

    public static readonly List<string> OutOfWaterTeleporters = new()
    {
        "guntp",
        "cragfieldtp",
        "kooshzonetp",
        "lostrivertp",
        "mushroomforesttp"
    };

    public struct TeleportData
    {
        public Vector3 teleportPosition;
        public float teleportAngle;

        public TeleportData(Vector3 teleportPosition, float teleportAngle)
        {
            this.teleportPosition = teleportPosition;
            this.teleportAngle = teleportAngle;
        }
    }
}
