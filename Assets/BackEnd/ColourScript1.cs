using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourScript1 : MonoBehaviour
{
    [SerializeField] private MaterialData[] materialArray;

    [System.Serializable]
    public class MaterialData
    {
        public Material[] materialArray;
        public Renderer renderer; // Use Renderer type to accept both MeshRenderer and SkinnedMeshRenderer
        public int currentMaterialIndex; // Store the current material index

        public Material GetCurrentMaterial()
        {
            return materialArray[currentMaterialIndex];
        }

        public void SetNextMaterial()
        {
            int previousIndex = currentMaterialIndex;
            currentMaterialIndex = (currentMaterialIndex + 1) % materialArray.Length;
            Debug.Log($"Changed material index from {previousIndex} to {currentMaterialIndex}");
        }

    }

    private void Awake()
    {
        LoadMaterialIndices();
    }

    public void ChangeMaterial()
    {
        Debug.Log("Changing materials");
        foreach (MaterialData materialData in materialArray)
        {
            materialData.SetNextMaterial(); // Update the current material index
            ApplyMaterial(materialData);
        }
        SaveMaterialIndices(); // Save every time the material changes
    }


    private void ApplyMaterial(MaterialData materialData)
    {
        if (materialData.renderer != null)
        {
            materialData.renderer.material = materialData.GetCurrentMaterial();
            Debug.Log($"Applied Material: {materialData.renderer.material.name} to {materialData.renderer.name}");
        }
        else
        {
            Debug.LogWarning("Renderer is not assigned.");
        }
    }


    private void SaveMaterialIndices()
    {
        for (int i = 0; i < materialArray.Length; i++)
        {
            string key = "MaterialIndex_" + gameObject.name + "_" + i;
            PlayerPrefs.SetInt(key, materialArray[i].currentMaterialIndex);
        }
        PlayerPrefs.Save();
    }

    private void LoadMaterialIndices()
    {
        for (int i = 0; i < materialArray.Length; i++)
        {
            int savedIndex = PlayerPrefs.GetInt("MaterialIndex_" + i, 0); // Default to 0 if not previously saved
            Debug.Log($"Loading saved material index {savedIndex} for renderer {materialArray[i].renderer.name}");
            if (savedIndex >= 0 && savedIndex < materialArray[i].materialArray.Length)
            {
                materialArray[i].currentMaterialIndex = savedIndex;
                ApplyMaterial(materialArray[i]); // Apply the saved material
            }
            else
            {
                Debug.LogError($"Saved material index {savedIndex} is out of range for renderer {i}");
            }
        }
    }


}
