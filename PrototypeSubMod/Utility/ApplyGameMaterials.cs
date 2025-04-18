using UnityEngine;

namespace PrototypeSubMod.Utility;

public static class ApplyGameMaterials
{
    public static void ApplyFromOriginal(GameObject originalPrefab, GameObject customPrefab)
    {
        if (customPrefab is null || originalPrefab is null)
        {
            Plugin.Logger.LogError("Couldn't apply game materials because a null object was passed into the method.");
            return;
        }
        
        var customRenderers = customPrefab.GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < customRenderers.Length; i++)
        {
            var newMaterialList = customRenderers[i].materials;

            for (int j = 0; j < newMaterialList.Length; j++)
            {
                bool matFound = false;

                foreach (var baseGameRenderer in originalPrefab.GetComponentsInChildren<Renderer>(true))
                {
                    foreach (var baseGameMat in baseGameRenderer.materials)
                    {
                        string baseMatName = baseGameMat.name.ToLower();
                        string plainBaseMatName = "";
                        
                        //Removes the (instance) portion of the mat name, if it's present.
                        if (baseMatName.Contains("(instance)"))
                            plainBaseMatName = baseMatName.Substring(0, baseMatName.LastIndexOf('(') - 1);

                        string customMatName = newMaterialList[j].name.ToLower();
                        if (customMatName.Equals(baseMatName) || customMatName.Equals(plainBaseMatName))
                        {
                            newMaterialList[j] = baseGameMat;
                            
                            matFound = true;
                            break;
                        }
                    }

                    if (matFound)
                        break;
                }
            }
            
            customRenderers[i].materials = newMaterialList;
        }
    }
}