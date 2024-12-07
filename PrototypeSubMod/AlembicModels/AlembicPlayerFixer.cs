using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

namespace PrototypeSubMod.AlembicModels;

internal class AlembicPlayerFixer : MonoBehaviour
{
    [SerializeField] private AlembicStreamPlayer player;
    [SerializeField] private string bytesModelName;

    private void Awake()
    {
        var model = Plugin.AssetBundle.LoadAsset<TextAsset>(bytesModelName);

        player.SetAllemData(model.bytes);
        player.Settings.ScaleFactor = 1;
    }
}
