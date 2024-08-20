using UnityEngine;
using System.Collections.Generic;

public class MaterialLister : MonoBehaviour
{
    private List<Material> allMaterials = new List<Material>();
    private Dictionary<Renderer, string> rendererToDesignGroup = new Dictionary<Renderer, string>();
    private Dictionary<string, List<Renderer>> designGroupToRenderers = new Dictionary<string, List<Renderer>>();

    void Start()
    {
        // Find all GameObjects with ChangeMaterial1 script attached
        ChangeMaterial1[] changeMaterialScripts = FindObjectsOfType<ChangeMaterial1>();

        // Iterate through each ChangeMaterial1 instance
        foreach (ChangeMaterial1 script in changeMaterialScripts)
        {
            if (script.materials != null)
            {
                // Iterate through each material in the script
                foreach (Material material in script.materials)
                {
                    // Add material to the list if not already present
                    if (!allMaterials.Contains(material))
                    {
                        allMaterials.Add(material);
                    }
                }

                // Link renderer to design group
                if (script.targetRenderer != null)
                {
                    string designGroupName = script.gameObject.name;

                    // Add to renderer to design group dictionary
                    if (!rendererToDesignGroup.ContainsKey(script.targetRenderer))
                    {
                        rendererToDesignGroup.Add(script.targetRenderer, designGroupName);
                    }

                    // Add to design group to renderers dictionary
                    if (!designGroupToRenderers.ContainsKey(designGroupName))
                    {
                        designGroupToRenderers[designGroupName] = new List<Renderer>();
                    }
                    designGroupToRenderers[designGroupName].Add(script.targetRenderer);
                }
            }
        }

        // Print out all materials found
        Debug.Log("Materials List:");
        foreach (Material material in allMaterials)
        {
            Debug.Log(material.name);
        }

        // Print out renderers and their associated design groups
        Debug.Log("Renderers and Design Groups:");
        foreach (var pair in rendererToDesignGroup)
        {
            Debug.Log($"{pair.Key.gameObject.name} -> {pair.Value}");
        }

        // Print out detailed design information
        Debug.Log("Detailed Design Information:");
        foreach (var designGroup in designGroupToRenderers)
        {
            Debug.Log($"Design Group: {designGroup.Key}");
            foreach (Renderer renderer in designGroup.Value)
            {
                Debug.Log($"  Renderer: {renderer.gameObject.name}");
                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    Debug.Log($"    Material {i + 1}: {materials[i].name}");
                }
            }
        }
    }
}
