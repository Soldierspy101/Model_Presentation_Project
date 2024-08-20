/*using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class HotspotEditorWindow : EditorWindow
{
    private List<HotspotData> dots = new List<HotspotData>();
    private HotspotData selectedHotspot;

    [MenuItem("Tools/Hotspot Editor")]
    public static void ShowWindow()
    {
        HotspotEditorWindow window = GetWindow<HotspotEditorWindow>("Hotspot Editor");
        window.LoadHotspots();
    }

    private void OnGUI()
    {
        GUILayout.Label("All Hotspots", EditorStyles.boldLabel);

        List<HotspotData> dotsCopy = new List<HotspotData>(dots);
        foreach (HotspotData dotData in dotsCopy)
        {
            try
            {
                GUILayout.BeginHorizontal();
                try
                {
                    if (GUILayout.Button(dotData.name))
                    {
                        OpenHotspotEditor(dotData);
                    }

                    GUILayout.Label($"ID: {dotData.dot.GetInstanceID()}");

                    if (GUILayout.Button("Remove"))
                    {
                        RemoveHotspot(dotData);
                        break;
                    }
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }
            }
            catch (MissingReferenceException)
            {
                dots.Remove(dotData);
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Spawn Hotspot"))
        {
            SpawnHotspot();
        }
    }

    private void SpawnHotspot()
    {
        GameObject newHotspot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newHotspot.transform.position = Vector3.zero; // Default position, can be changed in HotspotEditWindow
        newHotspot.transform.localScale = Vector3.one * 0.05f;

        // Create a new material instance for the hotspot
        Material hotspotMaterial = new Material(Shader.Find("Standard"));
        hotspotMaterial.color = Color.red; // Default color, can be changed in HotspotEditWindow
        newHotspot.GetComponent<Renderer>().sharedMaterial = hotspotMaterial;

        newHotspot.name = "Hotspot"; // Default name, can be changed in HotspotEditWindow

        HotspotData dotData = new HotspotData(newHotspot, newHotspot.transform.position, hotspotMaterial, "Hotspot", null);
        dots.Add(dotData);

        // Ensure the dot is marked as dirty so it gets saved with the scene
        EditorUtility.SetDirty(newHotspot);
        SaveHotspots();
    }


    private void RemoveHotspot(HotspotData dotData)
    {
        if (dotData.dot != null)
        {
            DestroyImmediate(dotData.dot);
        }
        dots.Remove(dotData);
        SaveHotspots();

        if (selectedHotspot == dotData)
        {
            selectedHotspot = null;
        }
    }

    private void OpenHotspotEditor(HotspotData dotData)
    {

        HotspotEditWindow.ShowWindow(dotData);
        selectedHotspot = dotData;
        Selection.activeGameObject = dotData.dot;
        Tools.current = Tool.Move;
    }

    private Vector3 ClampPositionToRendererBounds(Vector3 position, Renderer renderer)
    {
        Bounds bounds = renderer.bounds;
        position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        position.y = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
        position.z = Mathf.Clamp(position.z, bounds.min.z, bounds.max.z);
        return position;

    }

    private void UpdateHotspotBounds()
    {
        foreach (HotspotData dotData in dots)
        {
            if (dotData.targetRenderer != null)
            {
                dotData.bounds = dotData.targetRenderer.bounds;
            }
        }
    }


    private void OnSceneGUI(SceneView sceneView)
    {
        UpdateHotspotBounds();

        if (selectedHotspot != null)
        {
            if (selectedHotspot.dot == null)
            {
                selectedHotspot = null;
                return;
            }

            if (selectedHotspot.targetRenderer != null)
            {
                // Draw handles for the selected hotspot
                EditorGUI.BeginChangeCheck();
                Vector3 newPosition = Handles.PositionHandle(selectedHotspot.dot.transform.position, Quaternion.identity);
                Undo.RecordObject(selectedHotspot.dot.transform, "Move Hotspot");
                newPosition = ClampPositionToRendererBounds(newPosition, selectedHotspot.targetRenderer);
                selectedHotspot.dot.transform.position = newPosition;
                selectedHotspot.position = newPosition;
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(selectedHotspot.dot);
                }
            }
        }
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        LoadHotspots();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SaveHotspots();
    }

    private void SaveHotspots()
    {
        HotspotEditorSaveData saveData = new HotspotEditorSaveData { dots = dots };
        string json = JsonUtility.ToJson(saveData);
        EditorPrefs.SetString("HotspotEditorWindow_Hotspots", json);
    }

    private void LoadHotspots()
    {
        string json = EditorPrefs.GetString("HotspotEditorWindow_Hotspots", "");
        if (!string.IsNullOrEmpty(json))
        {
            HotspotEditorSaveData saveData = JsonUtility.FromJson<HotspotEditorSaveData>(json);
            dots = saveData.dots;

            foreach (HotspotData dotData in dots)
            {
                dotData.dot = GameObject.Find(dotData.name);
                if (dotData.dot != null)
                {
                    dotData.instanceID = dotData.dot.GetInstanceID();
                }
            }
        }
    }
}

[System.Serializable]
public class HotspotData
{
    public GameObject dot;
    public Vector3 position;
    public string name;
    public Renderer targetRenderer;
    public Bounds bounds;
    public int instanceID;

    // Use Material instead of Color for hotspot color
    public Material material;

    public HotspotData(GameObject dot, Vector3 position, Material material, string name, Renderer targetRenderer)
    {
        this.dot = dot;
        this.position = position;
        this.material = material;
        this.name = name;
        this.targetRenderer = targetRenderer;
        this.instanceID = dot.GetInstanceID();
        if (targetRenderer != null)
        {
            this.bounds = targetRenderer.bounds;
        }
    }
}


[System.Serializable]
public class HotspotEditorSaveData
{
    public List<HotspotData> dots;
}

public static class HotspotDataExtensions
{
    public static void UpdateHotspot(this HotspotData dotData)
    {
        if (dotData.dot != null)
        {
            dotData.dot.transform.position = dotData.position;
            dotData.dot.GetComponent<Renderer>().sharedMaterial = dotData.material; // Assign the material
            dotData.dot.name = dotData.name;
            EditorUtility.SetDirty(dotData.dot);
        }
    }
}

public class HotspotEditWindow : EditorWindow
{
    private HotspotData dotData;
    private Material initialMaterial;

    // Modify the ShowWindow method to pass the list of HotspotData objects
    public static void ShowWindow(HotspotData dotData)
    {
        HotspotEditWindow window = GetWindow<HotspotEditWindow>("Edit Hotspot");
        window.dotData = dotData;
        window.initialMaterial = dotData.material; // Save the initial material
        window.Show();
    }

    private void OnGUI()
    {
        if (dotData == null)
        {
            EditorGUILayout.HelpBox("No Hotspot selected.", MessageType.Warning);
            return;
        }

        GUILayout.Label("Edit Hotspot", EditorStyles.boldLabel);

        // Material Field for Hotspot Material with preview
        GUILayout.Label("Material", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        dotData.material = EditorGUILayout.ObjectField("Hotspot Material", dotData.material, typeof(Material), false) as Material;
        if (EditorGUI.EndChangeCheck())
        {
            UpdateHotspotMaterialPreview(dotData.material);
        }

        // Text Field for Hotspot Name
        GUILayout.Label("Name", EditorStyles.boldLabel);
        dotData.name = EditorGUILayout.TextField("Hotspot Name", dotData.name);

        // Field for selecting the target renderer
        GUILayout.Label("Target Renderer", EditorStyles.boldLabel);
        dotData.targetRenderer = EditorGUILayout.ObjectField("Target Renderer", dotData.targetRenderer, typeof(Renderer), true) as Renderer;

        // Display current position (not editable here, updated in SceneView)
        GUILayout.Label("Position", EditorStyles.boldLabel);
        EditorGUILayout.Vector3Field("Hotspot Position", dotData.dot.transform.position);
        if (dotData.targetRenderer == null)
        {
            EditorGUILayout.HelpBox("Please assign a target renderer.", MessageType.Error);
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Update Hotspot"))
        {
            if (dotData.targetRenderer == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a target renderer before updating the dot.", "OK");
                return;
            }

            // Ensure properties are updated
            dotData.position = dotData.dot.transform.position; // Ensure dotData position is up-to-date
            dotData.UpdateHotspot();
            Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            // Revert to the initial material if the user cancels
            dotData.material = initialMaterial;
            dotData.UpdateHotspot();
            Close();
        }

        GUILayout.EndHorizontal();
    }

    // Update the hotspot material preview
    private void UpdateHotspotMaterialPreview(Material material)
    {
        dotData.dot.GetComponent<Renderer>().sharedMaterial = material;
        EditorUtility.SetDirty(dotData.dot);
    }
}
*/