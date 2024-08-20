using UnityEngine;
using UnityEditor;
using System.IO;

public class FBXMaterialSetup : AssetPostprocessor
{
    ModelImporter modelImporter;
    void OnPostprocessModel(GameObject g)
    {
        // Ensure this is an FBX file
        if (Path.GetExtension(assetPath).ToLower() == ".fbx")
        {
            // Extract materials and textures
            ExtractMaterialsAndTextures(assetPath);
        }
    }
    void OnPostprocessMaterial(Material m)
    {
        Debug.Log(m.name);
    }
    private void ExtractMaterialsAndTextures(string assetPath)
    {
        string assetDirectory = Path.GetDirectoryName(assetPath);
        string modelName = Path.GetFileNameWithoutExtension(assetPath);

        string texturesDirectory = Path.Combine(assetDirectory, modelName + "_Textures");

        if (!Directory.Exists(texturesDirectory))
        {
            Directory.CreateDirectory(texturesDirectory);
        }

        modelImporter = assetImporter as ModelImporter;
        if (modelImporter != null)
        {
            modelImporter.ExtractTextures(texturesDirectory);
            modelImporter.materialSearch = ModelImporterMaterialSearch.RecursiveUp;
            modelImporter.materialLocation = ModelImporterMaterialLocation.InPrefab;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

    }
}
