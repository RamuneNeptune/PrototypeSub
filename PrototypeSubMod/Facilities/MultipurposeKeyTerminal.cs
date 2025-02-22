using Nautilus.Handlers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UWE;
using static PrecursorKeyTerminal;

namespace PrototypeSubMod.Facilities;

internal class MultipurposeKeyTerminal : MonoBehaviour
{
    private static GameObject KeyTerminalPrefab;

    [SerializeField] private string techType;
    [SerializeField] private Texture2D replacementSprite;
    [SerializeField] private UnityEvent onInteracted;

    private IEnumerator Start()
    {
        if (KeyTerminalPrefab)
        {
            SpawnPrefab(KeyTerminalPrefab);
            yield break;
        }

        var prefabRequest = PrefabDatabase.GetPrefabAsync("c718547d-fe06-4247-86d0-efd1e3747af0");

        yield return prefabRequest;

        GameObject prefab = null;
        if (!prefabRequest.TryGetPrefab(out prefab)) throw new Exception("Error retrieving precursor key terminal prefab!");

        prefab.SetActive(false);
        KeyTerminalPrefab = prefab;
        SpawnPrefab(KeyTerminalPrefab);
    }

    private void SpawnPrefab(GameObject prefab)
    {
        TechType techType = TechType.None;
        try
        {
            techType = (TechType)Enum.Parse(typeof(TechType), this.techType);
        }
        catch (Exception e)
        {
            throw e;
        }

        var keyType = CreateOrRetrieveKeyType(techType);
        var instance = Instantiate(prefab, transform, false);

        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        instance.SetActive(true);

        var keyTerminal = instance.GetComponent<PrecursorKeyTerminal>();
        keyTerminal.acceptKeyType = keyType;
        var glyphRenderer = keyTerminal.transform.Find("Precursor_key_terminal_01/glyph/Face_F").GetComponent<Renderer>();

        glyphRenderer.material.mainTexture = replacementSprite;

        Destroy(instance.GetComponent<PrefabIdentifier>());
    }

    private PrecursorKeyType CreateOrRetrieveKeyType(TechType type)
    {
        PrecursorKeyType value;
        try
        {
            value = (PrecursorKeyType)Enum.Parse(typeof(PrecursorKeyType), type.ToString());
            return value;
        }
        catch
        {
            var builder = EnumHandler.AddEntry<PrecursorKeyType>(type.ToString());
            return builder.Value;
        }
    }

    // Called via BroadcastMessage on PrecursorKeyTerminal
    public void ToggleDoor()
    {
        onInteracted?.Invoke();
    }
}
