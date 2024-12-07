using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

namespace PrototypeSubMod.AlembicModels;

internal class AlembicAnimPlayer : MonoBehaviour
{
    [SerializeField] private AlembicStreamPlayer alemPlayer;
    [SerializeField] private float playbackSpeed = 1;

    private void Update()
    {
        alemPlayer.CurrentTime += playbackSpeed * Time.deltaTime;
        alemPlayer.CurrentTime = alemPlayer.CurrentTime % alemPlayer.EndTime;
    }
}
