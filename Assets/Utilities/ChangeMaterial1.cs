using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]

public class ChangeMaterial1 : MonoBehaviour
{
    public Renderer targetRenderer;
    public int currentMaterialIndex;  // Ensure this is defined
    public string targetRendererName;
    public Material[] materials;
    [System.NonSerialized] public string targetRendererPath; // This will be used for serialization
    [System.NonSerialized] public List<string> materialPaths = new List<string>();

    public Material selectedMaterial;

    public Button changeMaterialButton;
    public Button changeMaterialBackwardsButton;
    private bool isButtonInitialized = false;
    public bool materialsAutofilled = false;

    private int maxDesignCount; // Maximum number of designs among all target renderers
    private Dictionary<Renderer, List<Material>> rendererToMaterials = new Dictionary<Renderer, List<Material>>(); // To store materials for each renderer

    private void Start()
    {
        InitializeButtons();
        FindTargetRenderers(); // Find and initialize target renderers and their materials
        CalculateMaxDesignCount(); // Calculate the maximum number of designs across all target renderers
        LoadCurrentDesignIndex(); // Load the saved design index
        ApplyDesign(); // Apply the loaded design immediately
    }

    private void Update()
    {
        if (!isButtonInitialized)
        {
            InitializeButtons();
        }
    }
#if UNITY_EDITOR
    public void UpdateMaterialPaths()
    {
        materialPaths.Clear();
        foreach (var material in materials)
        {
            if (material != null)
            {
                materialPaths.Add(AssetDatabase.GetAssetPath(material));
            }
        }
    }

    public void SetMaterialsFromPaths()
    {
        materials = materialPaths.Select(path => AssetDatabase.LoadAssetAtPath<Material>(path)).ToArray();
    }
    // Update target renderer path
    public void UpdateTargetRendererPath()
    {
        if (targetRenderer != null)
        {
            targetRendererPath = GetGameObjectPath(targetRenderer.gameObject);
        }
    }

    // Update material paths
    public List<string> GetMaterialPaths()
    {
        List<string> paths = new List<string>();
        foreach (var material in materials)
        {
            if (material != null)
            {
                paths.Add(AssetDatabase.GetAssetPath(material));
            }
        }
        return paths;
    }


    // Set materials from paths
    public void SetMaterialsFromPaths(List<string> paths)
    {
        materials = paths.Select(path => AssetDatabase.LoadAssetAtPath<Material>(path)).ToArray();
    }

    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = obj.name + "/" + path;
        }
        return path;
    }

#endif
    public Renderer FindRendererByPath(string path)
    {
        GameObject obj = GameObject.Find(path);
        return obj ? obj.GetComponent<Renderer>() : null;
    }
    private void InitializeButtons()
    {
        // Navigate to find the buttons
        GameObject uiGameObject = GameObject.Find("Functions");
        if (uiGameObject != null)
        {
            GameObject FourthFunc = GameObject.Find("FourthFunction");
            if (FourthFunc != null)
            {
                // Initialize forward button
                Transform forwardButtonTransform = FourthFunc.transform.Find("ChooseDesign");
                if (forwardButtonTransform != null)
                {
                    changeMaterialButton = forwardButtonTransform.GetComponent<Button>();
                    if (changeMaterialButton != null)
                    {
                        // Adding listener to your button
                        changeMaterialButton.onClick.AddListener(OnChangeMaterialButtonClicked);
                        Debug.Log("Listener added to ChooseDesign button.");
                    }
                }
                else
                {
                    Debug.LogWarning("ChooseDesign button not found or has a different name.");
                }

                // Initialize backward button
                Transform backwardButtonTransform = FourthFunc.transform.Find("BackChooseDesign");
                if (backwardButtonTransform != null)
                {
                    changeMaterialBackwardsButton = backwardButtonTransform.GetComponent<Button>();
                    if (changeMaterialBackwardsButton != null)
                    {
                        // Adding listener to your button
                        changeMaterialBackwardsButton.onClick.AddListener(OnChangeMaterialBackwardsButtonClicked);
                        Debug.Log("Listener added to BackChooseDesign button.");
                    }
                }
                else
                {
                    Debug.LogWarning("BackChooseDesign button not found or has a different name.");
                }

                isButtonInitialized = true; // Buttons are initialized, stop checking
            }
        }
        else
        {
            Debug.LogWarning("Functions object not found.");
        }


    }


    private void OnChangeMaterialButtonClicked()
    {
        currentMaterialIndex = (currentMaterialIndex + 1) % maxDesignCount;
        ApplyDesign();
        Debug.Log($"Applied design {currentMaterialIndex + 1} of {maxDesignCount}");
        SaveCurrentDesignIndex();
    }

    private void OnChangeMaterialBackwardsButtonClicked()
    {
        currentMaterialIndex = (currentMaterialIndex - 1 + maxDesignCount) % maxDesignCount;
        ApplyDesign();
        Debug.Log($"Applied design {currentMaterialIndex - 1} of {maxDesignCount}");
        SaveCurrentDesignIndex();
    }

    private void ApplyDesign()
    {
        if (targetRenderer != null && materials != null && materials.Length > 0)
        {
            int defaultMaterialCount = targetRenderer.sharedMaterials.Length;
            int startIndex = currentMaterialIndex * defaultMaterialCount;

            Material[] newMaterials = new Material[defaultMaterialCount];
            for (int i = 0; i < defaultMaterialCount; i++)
            {
                newMaterials[i] = materials[startIndex + i];
            }
            
           
            targetRenderer.materials = newMaterials;
            
        }
    }

    private void SaveCurrentDesignIndex()
    {
        PlayerPrefs.SetInt($"{gameObject.name}_DesignIndex", currentMaterialIndex);
        PlayerPrefs.Save();
    }

    private void LoadCurrentDesignIndex()
    {
        currentMaterialIndex = PlayerPrefs.GetInt($"{gameObject.name}_DesignIndex", 0);
    }

    private void FindTargetRenderers()
    {
        GameObject dragOptionsHere = GameObject.Find("DragOptionsHere");

        if (dragOptionsHere == null)
        {
            Debug.LogWarning("DragOptionsHere not found.");
            return;
        }

        // Clear existing renderer to materials mapping
        rendererToMaterials.Clear();

        // Find all ChangeMaterial1 scripts on DragOptionsHere GameObject
        ChangeMaterial1[] changeMaterialScripts = dragOptionsHere.GetComponentsInChildren<ChangeMaterial1>();

        // Populate renderer to materials dictionary
        foreach (ChangeMaterial1 script in changeMaterialScripts)
        {
            Renderer renderer = script.targetRenderer;
            if (renderer != null)
            {
                if (!rendererToMaterials.ContainsKey(renderer))
                {
                    rendererToMaterials[renderer] = new List<Material>();
                }

                // Add unique materials assigned to this renderer
                foreach (Material material in script.materials)
                {
                    if (!rendererToMaterials[renderer].Contains(material))
                    {
                        rendererToMaterials[renderer].Add(material);
                    }
                }
            }
        }
    }

    private void CalculateMaxDesignCount()
    {
        maxDesignCount = 0;

        foreach (var kvp in rendererToMaterials)
        {
            Renderer renderer = kvp.Key;
            List<Material> materialsList = kvp.Value;

            int defaultMaterialCount = renderer.sharedMaterials.Length;
            int designCount = materialsList.Count / defaultMaterialCount;
            maxDesignCount = Mathf.Max(maxDesignCount, designCount);
        }
    }
}

