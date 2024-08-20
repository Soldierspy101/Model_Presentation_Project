/*
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class AnnotationWindow : EditorWindow
{
    private GameObject textPrefab;
    public Renderer targetRenderer;
    private string annotationText;
    private Vector3 offset = Vector3.up * 1;
    private Vector3 rotation = new Vector3(0, 90, 0); // Initial Y rotation set to 90 degrees
    private Material lineMaterial;
    private Canvas targetCanvas;
    private List<AnnotationData> annotations = new List<AnnotationData>();
    private AnnotationData selectedAnnotation;
    private GameObject previewText;
    private LineRenderer previewLine;

    private const string AnnotationsKey = "AnnotationsData";

    [MenuItem("Tools/Annotation Editor")]
    public static void ShowWindow()
    {
        AnnotationWindow window = GetWindow<AnnotationWindow>("Annotation Editor");
        LoadAnnotations(window); // Load annotations data when window is opened
        Canvas canvas = GameObject.Find("ModelCanvas")?.GetComponent<Canvas>();

        if (canvas != null)
        {
            window.targetCanvas = canvas;
            TextMeshProUGUI textPrefab = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (textPrefab != null)
            {
                window.textPrefab = textPrefab.gameObject;
            }
            else
            {
                Debug.LogWarning("Text (TMP) object not found as a child of the ModelCanvas. Please ensure Text (TMP) object is a child of the ModelCanvas prefab.");
            }
        }
        else
        {
            Debug.LogWarning("ModelCanvas object not found in the scene. Please ensure a ModelCanvas object with the name 'ModelCanvas' is present in the scene.");
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Add Annotation", EditorStyles.boldLabel);

        bool missingFields = textPrefab == null || targetRenderer == null || lineMaterial == null || targetCanvas == null;

        if (missingFields)
        {
            EditorGUILayout.HelpBox("Please fill in all required fields.", MessageType.Warning);
        }
        targetRenderer = EditorGUILayout.ObjectField("Target Renderer", targetRenderer, typeof(Renderer), true) as Renderer;
        lineMaterial = EditorGUILayout.ObjectField("Line Material", lineMaterial, typeof(Material), true) as Material;
        annotationText = EditorGUILayout.TextField("Annotation Text", annotationText);

        GUILayout.Label("Offset", EditorStyles.boldLabel);
        offset.x = EditorGUILayout.Slider("X", Mathf.Round(offset.x * 10f) / 10f, -10f, 10f);
        offset.y = EditorGUILayout.Slider("Y", Mathf.Round(offset.y * 10f) / 10f, -10f, 10f);
        offset.z = EditorGUILayout.Slider("Z", Mathf.Round(offset.z * 10f) / 10f, -10f, 10f);

        GUILayout.Label("Rotation", EditorStyles.boldLabel);
        rotation.x = EditorGUILayout.Slider("X", Mathf.Round(rotation.x * 10f) / 10f, -180f, 180f);
        rotation.y = EditorGUILayout.Slider("Y", Mathf.Round(rotation.y * 10f) / 10f, -180f, 180f);
        rotation.z = EditorGUILayout.Slider("Z", Mathf.Round(rotation.z * 10f) / 10f, -180f, 180f);

        if (GUILayout.Button("Save") && !missingFields)
        {
            if (targetRenderer is MeshRenderer || targetRenderer is SkinnedMeshRenderer)
            {
                Debug.Log("Save Successful");
                AnnotationData newAnnotation = new AnnotationData() { ownText = annotationText };
                AddAnnotation(newAnnotation, offset, rotation, targetCanvas);
                annotations.Add(newAnnotation); // Ensure the annotation is added to the list
                ResetFields();
                DestroyPreview();
                SaveAnnotations();
            }
            else
            {
                Debug.LogWarning("Target Renderer must be either a MeshRenderer or a SkinnedMeshRenderer.");
            }
        }

        if (GUILayout.Button("Cancel"))
        {
            Debug.Log("Cancel Successful");
            ResetFields();
            DestroyPreview();
        }

        GUILayout.Label("Annotations", EditorStyles.boldLabel);
        foreach (AnnotationData annotation in new List<AnnotationData>(annotations))
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(annotation.ownText, EditorStyles.label);
            if (GUILayout.Button("Edit"))
            {
                selectedAnnotation = annotation;
                ShowEditWindow();
            }
            if (GUILayout.Button("Remove"))
            {
                RemoveAnnotation(annotation);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed && !missingFields)
        {
            DrawAnnotationPreview(offset, rotation, annotationText);
        }

        if (GUILayout.Button("List All Annotations"))
        {
            ListAllAnnotationsInHierarchy();
        }
    }

    private void OnDestroy()
    {
        SaveAnnotations(); // Save annotations data when window is closed
    }

    private static void LoadAnnotations(AnnotationWindow window)
    {
        string json = EditorPrefs.GetString(AnnotationsKey);
        if (!string.IsNullOrEmpty(json))
        {
            window.annotations = JsonUtility.FromJson<AnnotationDataList>(json).annotations;
            foreach (var annotation in window.annotations)
            {
                Renderer renderer = annotation.GetTargetRenderer();
                if (renderer != null)
                {
                    Debug.Log("Loaded renderer instance ID: " + annotation.rendererInstanceID);
                }
                else
                {
                    Debug.LogWarning("Failed to load renderer for instance ID: " + annotation.rendererInstanceID);
                }
            }
        }
    }


    private void SaveAnnotations()
    {
        Debug.Log("Annotations Saved");
        foreach (var annotation in annotations)
        {
            if (annotation.targetRenderer != null)
            {
                annotation.rendererInstanceID = annotation.targetRenderer.GetInstanceID();
                Debug.Log("Saving renderer instance ID: " + annotation.rendererInstanceID);
            }
        }
        string json = JsonUtility.ToJson(new AnnotationDataList { annotations = annotations });
        EditorPrefs.SetString(AnnotationsKey, json);
    }


    private void ShowEditWindow()
    {
        AnnotationEditWindow editWindow = ScriptableObject.CreateInstance<AnnotationEditWindow>();
        editWindow.Init(this, selectedAnnotation, lineMaterial, textPrefab, targetCanvas);
        editWindow.ShowUtility();
    }


    private void AddAnnotation(AnnotationData annotationData, Vector3 offset, Vector3 rotation, Canvas canvas)
    {
        Debug.Log("Adding annotation for: " + targetRenderer.gameObject.name);
        Bounds bounds;

        if (targetRenderer is MeshRenderer)
        {
            bounds = ((MeshRenderer)targetRenderer).bounds;
        }
        else if (targetRenderer is SkinnedMeshRenderer)
        {
            bounds = ((SkinnedMeshRenderer)targetRenderer).bounds;
        }
        else
        {
            Debug.LogWarning("Unknown renderer type. Cannot add annotation.");
            return;
        }

        Vector3 annotationPosition = bounds.center + offset;

        if (textPrefab != null)
        {
            GameObject textInstance = Instantiate(textPrefab, canvas.transform);
            TextMeshProUGUI textComponent = textInstance.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                annotationData.text = textComponent;
                annotationData.text.rectTransform.position = annotationPosition;
                annotationData.text.rectTransform.rotation = Quaternion.Euler(rotation);
                annotationData.text.text = annotationData.ownText;
                annotationData.SetTargetRenderer(targetRenderer); // Store the targetRenderer in annotationData
                Debug.Log("Annotation text set to: " + annotationData.ownText);
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found in the instantiated prefab. Cannot add annotation.");
                return;
            }

            GameObject lineObj = new GameObject("Line");
            lineObj.transform.SetParent(annotationData.text.transform, false);
            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.SetPosition(0, bounds.center);
            lineRenderer.SetPosition(1, annotationPosition);

            annotationData.position = annotationPosition;
            annotationData.rotation = rotation;
        }
        else
        {
            Debug.LogWarning("TextPrefab is null. Cannot add annotation.");
            return;
        }
    }

    private void ResetFields()
    {
        targetRenderer = null;
        lineMaterial = null;
        annotationText = null;
        offset = Vector3.up * 1;
        rotation = new Vector3(0, 90, 0);
    }

    private void RemoveAnnotation(AnnotationData annotationData)
    {
        DestroyImmediate(annotationData.text.gameObject);
        annotations.Remove(annotationData);
    }

    public void UpdateAnnotation(AnnotationData annotation, string newText, Vector3 newPosition, Vector3 newRotation)
    {



        LineRenderer lineRenderer = annotation.text.transform.Find("Line").GetComponent<LineRenderer>();
        Bounds bounds;

        if (annotation.targetRenderer is MeshRenderer)
        {
            bounds = ((MeshRenderer)annotation.targetRenderer).bounds;
        }
        else if (annotation.targetRenderer is SkinnedMeshRenderer)
        {
            bounds = ((SkinnedMeshRenderer)annotation.targetRenderer).bounds;
        }
        else
        {
            Debug.LogWarning("Unknown renderer type. Cannot update annotation.");
            return;
        }
        // Update line renderer position
        Vector3 annotationPosition = newPosition;
        Vector3 annotationPotation1 = bounds.center + newPosition;
        annotation.ownText = newText;
        annotation.text.text = newText;

        // Update position and rotation directly
        annotation.position = annotationPosition;
        annotation.text.rectTransform.position = annotationPotation1;
        annotation.rotation = newRotation;
        annotation.text.rectTransform.rotation = Quaternion.Euler(newRotation);
        // Calculate line position

        lineRenderer.SetPosition(1, annotationPotation1);
    }




    public void DrawAnnotationPreview(Vector3 position, Vector3 rotation, string text)
    {
        if (previewText == null)
        {
            previewText = Instantiate(textPrefab, targetCanvas.transform);
            previewText.name = "PreviewText";
        }

        Bounds bounds;

        if (targetRenderer is MeshRenderer)
        {
            bounds = ((MeshRenderer)targetRenderer).bounds;
        }
        else if (targetRenderer is SkinnedMeshRenderer)
        {
            bounds = ((SkinnedMeshRenderer)targetRenderer).bounds;
        }
        else
        {
            return;
        }

        Vector3 annotationPosition = bounds.center + position;
        previewText.GetComponent<TextMeshProUGUI>().text = text;
        previewText.GetComponent<RectTransform>().position = annotationPosition;
        previewText.GetComponent<RectTransform>().rotation = Quaternion.Euler(rotation);

        if (previewLine == null)
        {
            GameObject lineObj = new GameObject("LinePreview");
            lineObj.transform.SetParent(previewText.transform, false);
            previewLine = lineObj.AddComponent<LineRenderer>();
            previewLine.material = lineMaterial;
            previewLine.startWidth = 0.02f;
            previewLine.endWidth = 0.02f;
        }

        previewLine.SetPosition(0, bounds.center);
        previewLine.SetPosition(1, annotationPosition);
    }

    public void DestroyPreview()
    {
        if (previewText != null)
        {
            DestroyImmediate(previewText);
        }

        if (previewLine != null)
        {
            DestroyImmediate(previewLine.gameObject);
        }
    }

    public void ListAllAnnotationsInHierarchy()
    {
        TextMeshProUGUI[] texts = GameObject.FindObjectsOfType<TextMeshProUGUI>();
        Dictionary<int, AnnotationData> existingAnnotations = new Dictionary<int, AnnotationData>();

        // Create a dictionary of existing annotations based on their text component instance ID
        foreach (var annotation in annotations)
        {
            if (annotation.text != null)
            {
                existingAnnotations[annotation.text.GetInstanceID()] = annotation;
            }
        }

        foreach (TextMeshProUGUI text in texts)
        {
            if (text.gameObject.name.Contains("(Clone)"))
            {
                int textInstanceID = text.GetInstanceID();
                if (!existingAnnotations.ContainsKey(textInstanceID))
                {
                    Renderer renderer = text.GetComponentInParent<Renderer>();
                    if (renderer != null)
                    {
                        AnnotationData annotation = new AnnotationData
                        {
                            text = text,
                            ownText = text.text,
                            position = text.rectTransform.position,
                            rotation = text.rectTransform.rotation.eulerAngles,
                            targetRenderer = renderer
                        };
                        annotation.SetTargetRenderer(renderer); // Ensure renderer instance ID is set
                        annotations.Add(annotation);
                    }
                    else
                    {
                        Debug.LogWarning("Renderer not found for text object: " + text.gameObject.name);
                    }
                }
                else
                {
                    // Update existing annotation data if necessary
                    existingAnnotations[textInstanceID].ownText = text.text;
                    existingAnnotations[textInstanceID].position = text.rectTransform.position;
                    existingAnnotations[textInstanceID].rotation = text.rectTransform.rotation.eulerAngles;
                }
            }
        }

        Debug.Log("Total Annotations found: " + annotations.Count);
        foreach (AnnotationData annotation in annotations)
        {
            Debug.Log("Annotation: " + annotation.ownText + " | Renderer Instance ID: " + (annotation.targetRenderer != null ? annotation.targetRenderer.GetInstanceID().ToString() : "null") + " | Text Instance ID: " + annotation.text.GetInstanceID());
        }
    }
}

[Serializable]
public class AnnotationData
{
    public TextMeshProUGUI text;
    public string ownText;
    public Vector3 position;
    public Vector3 rotation;
    public Renderer targetRenderer;
    public int rendererInstanceID;

    public void SetTargetRenderer(Renderer renderer)
    {
        targetRenderer = renderer;
        rendererInstanceID = renderer != null ? renderer.GetInstanceID() : 0;
    }

    public Renderer GetTargetRenderer()
    {
        if (targetRenderer == null && rendererInstanceID != 0)
        {
            targetRenderer = EditorUtility.InstanceIDToObject(rendererInstanceID) as Renderer;
        }
        return targetRenderer;
    }
}


[Serializable]
public class AnnotationDataList
{
    public List<AnnotationData> annotations;
}
public class AnnotationEditWindow : EditorWindow
{
    private AnnotationWindow parentWindow;
    private AnnotationData annotationData;
    private string editedText;
    private Vector3 editedPosition;
    private Vector3 editedRotation;

    private string originalText;
    private Vector3 originalPosition;
    private Vector3 originalRotation;

    private GameObject previewText;
    private LineRenderer previewLine;
    private Material lineMaterial;
    private GameObject textPrefab;
    private Canvas targetCanvas;

    public void Init(AnnotationWindow parent, AnnotationData annotation, Material lineMat, GameObject textPref, Canvas canvas)
    {
        parentWindow = parent;
        annotationData = annotation;
        lineMaterial = lineMat;
        textPrefab = textPref;
        targetCanvas = canvas;

        // Ensure targetRenderer is set
        annotationData.GetTargetRenderer();

        originalText = annotation.ownText;
        originalPosition = annotation.position;
        originalRotation = annotation.rotation;

        editedText = originalText;
        editedPosition = originalPosition;
        editedRotation = originalRotation;
    }


    private void OnGUI()
    {
        editedText = EditorGUILayout.TextField("Edit Annotation", editedText);

        GUILayout.Label("Edit Offset", EditorStyles.boldLabel);
        editedPosition.x = EditorGUILayout.Slider("X", Mathf.Round(editedPosition.x * 10f) / 10f, -10f, 10f);
        editedPosition.y = EditorGUILayout.Slider("Y", Mathf.Round(editedPosition.y * 10f) / 10f, -10f, 10f);
        editedPosition.z = EditorGUILayout.Slider("Z", Mathf.Round(editedPosition.z * 10f) / 10f, -10f, 10f);

        GUILayout.Label("Edit Rotation", EditorStyles.boldLabel);
        editedRotation.x = EditorGUILayout.Slider("X", Mathf.Round(editedRotation.x * 10f) / 10f, -180f, 180f);
        editedRotation.y = EditorGUILayout.Slider("Y", Mathf.Round(editedRotation.y * 10f) / 10f, -180f, 180f);
        editedRotation.z = EditorGUILayout.Slider("Z", Mathf.Round(editedRotation.z * 10f) / 10f, -180f, 180f);

        // Draw preview when fields change
        if (Event.current.type == EventType.Repaint)
        {
            DrawPreview(editedPosition, editedRotation, editedText); // Pass the required arguments
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("OK"))
        {
            parentWindow.UpdateAnnotation(annotationData, editedText, editedPosition, editedRotation);
            parentWindow.DestroyPreview();
            Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            parentWindow.DrawAnnotationPreview(originalPosition, originalRotation, originalText);
            parentWindow.DestroyPreview();
            Close();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPreview(Vector3 position, Vector3 rotation, string text)
    {
        // Ensure the targetRenderer is correctly retrieved
        if (annotationData.GetTargetRenderer() == null)
        {
            Debug.LogWarning("Target renderer is null. Cannot draw preview.");
            return;
        }

        Bounds bounds;

        if (annotationData.targetRenderer is MeshRenderer)
        {
            bounds = ((MeshRenderer)annotationData.targetRenderer).bounds;
        }
        else if (annotationData.targetRenderer is SkinnedMeshRenderer)
        {
            bounds = ((SkinnedMeshRenderer)annotationData.targetRenderer).bounds;
        }
        else
        {
            Debug.LogWarning("Unknown renderer type. Cannot add annotation.");
            return;
        }

        if (previewText == null)
        {
            previewText = Instantiate(textPrefab, targetCanvas.transform);
            previewText.name = "PreviewText";
        }

        Vector3 annotationPosition = bounds.center + position;
        TextMeshProUGUI textComponent = previewText.GetComponent<TextMeshProUGUI>();
        RectTransform rectTransform = previewText.GetComponent<RectTransform>();

        if (textComponent != null)
        {
            textComponent.text = text;
        }

        if (rectTransform != null)
        {
            rectTransform.position = annotationPosition;
            rectTransform.rotation = Quaternion.Euler(rotation);
        }

        if (previewLine == null)
        {
            GameObject lineObj = new GameObject("LinePreview");
            lineObj.transform.SetParent(previewText.transform, false);
            previewLine = lineObj.AddComponent<LineRenderer>();
            previewLine.material = lineMaterial;
            previewLine.startWidth = 0.02f;
            previewLine.endWidth = 0.02f;
        }

        previewLine.SetPosition(0, bounds.center);
        previewLine.SetPosition(1, annotationPosition);
    }

    private void OnDestroy()
    {
        if (previewText != null)
        {
            DestroyImmediate(previewText);
        }

        if (previewLine != null)
        {
            DestroyImmediate(previewLine.gameObject);
        }
    }
}
*/
/*
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnnotationWindow : EditorWindow
{
    private GameObject textPrefab;
    public Renderer targetRenderer;
    private string annotationText;
    private Vector3 offset = Vector3.up * 1;
    private Vector3 rotation = new Vector3(0, 90, 0); // Initial Y rotation set to 90 degrees
    private Material lineMaterial;
    private Canvas targetCanvas;
    private List<AnnotationData> annotations = new List<AnnotationData>();
    private AnnotationData selectedAnnotation;
    private GameObject previewText;
    private LineRenderer previewLine;

    private const string AnnotationsKey = "AnnotationsData";

    // List to keep track of spawned dots
    private List<GameObject> spawnedDots = new List<GameObject>();
    private GameObject selectedDot;

    [MenuItem("Tools/Annotation Editor")]
    public static void ShowWindow()
    {
        AnnotationWindow window = GetWindow<AnnotationWindow>("Annotation Editor");
        LoadAnnotations(window); // Load annotations data when window is opened
        Canvas canvas = GameObject.Find("ModelCanvas")?.GetComponent<Canvas>();

        if (canvas != null)
        {
            window.targetCanvas = canvas;
            TextMeshProUGUI textPrefab = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (textPrefab != null)
            {
                window.textPrefab = textPrefab.gameObject;
            }
            else
            {
                Debug.LogWarning("Text (TMP) object not found as a child of the ModelCanvas. Please ensure Text (TMP) object is a child of the ModelCanvas prefab.");
            }
        }
        else
        {
            Debug.LogWarning("ModelCanvas object not found in the scene. Please ensure a ModelCanvas object with the name 'ModelCanvas' is present in the scene.");
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Add Annotation", EditorStyles.boldLabel);

        bool missingFields = textPrefab == null || lineMaterial == null || targetCanvas == null;

        if (missingFields)
        {
            EditorGUILayout.HelpBox("Please fill in all required fields.", MessageType.Warning);
        }
        targetRenderer = EditorGUILayout.ObjectField("Target Renderer", targetRenderer, typeof(Renderer), true) as Renderer;
        lineMaterial = EditorGUILayout.ObjectField("Line Material", lineMaterial, typeof(Material), true) as Material;
        annotationText = EditorGUILayout.TextField("Annotation Text", annotationText);

        GUILayout.Label("Offset", EditorStyles.boldLabel);
        offset.x = EditorGUILayout.Slider("X", Mathf.Round(offset.x * 10f) / 10f, -10f, 10f);
        offset.y = EditorGUILayout.Slider("Y", Mathf.Round(offset.y * 10f) / 10f, -10f, 10f);
        offset.z = EditorGUILayout.Slider("Z", Mathf.Round(offset.z * 10f) / 10f, -10f, 10f);

        GUILayout.Label("Rotation", EditorStyles.boldLabel);
        rotation.x = EditorGUILayout.Slider("X", Mathf.Round(rotation.x * 10f) / 10f, -180f, 180f);
        rotation.y = EditorGUILayout.Slider("Y", Mathf.Round(rotation.y * 10f) / 10f, -180f, 180f);
        rotation.z = EditorGUILayout.Slider("Z", Mathf.Round(rotation.z * 10f) / 10f, -180f, 180f);

        if (GUILayout.Button("Save") && !missingFields)
        {
            if (targetRenderer != null && (targetRenderer is MeshRenderer || targetRenderer is SkinnedMeshRenderer))
            {
                Debug.Log("Save Successful");
                AnnotationData newAnnotation = new AnnotationData() { ownText = annotationText };
                AddAnnotation(newAnnotation, offset, rotation, targetCanvas);
                annotations.Add(newAnnotation); // Ensure the annotation is added to the list
                ResetFields();
                DestroyPreview();
                SaveAnnotations();
            }
            else if (selectedDot != null)
            {
                Debug.Log("Save Successful");
                AnnotationData newAnnotation = new AnnotationData() { ownText = annotationText };
                AddDotAnnotation(newAnnotation, selectedDot, offset, rotation, targetCanvas);
                annotations.Add(newAnnotation); // Ensure the annotation is added to the list
                ResetFields();
                DestroyPreview();
                SaveAnnotations();
            }
            else
            {
                Debug.LogWarning("Please select a target renderer or dot.");
            }
        }

        if (GUILayout.Button("Cancel"))
        {
            Debug.Log("Cancel Successful");
            ResetFields();
            DestroyPreview();
        }

        GUILayout.Label("Annotations", EditorStyles.boldLabel);
        foreach (AnnotationData annotation in new List<AnnotationData>(annotations))
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(annotation.ownText, EditorStyles.label);
            if (GUILayout.Button("Edit"))
            {
                selectedAnnotation = annotation;
                ShowEditWindow();
            }
            if (GUILayout.Button("Remove"))
            {
                RemoveAnnotation(annotation);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Spawn Dot"))
        {
            SpawnDot();
        }

        if (selectedDot != null)
        {
            GUILayout.Label("Move and Rotate Dot", EditorStyles.boldLabel);
            offset.x = EditorGUILayout.Slider("X", Mathf.Round(offset.x * 10f) / 10f, -10f, 10f);
            offset.y = EditorGUILayout.Slider("Y", Mathf.Round(offset.y * 10f) / 10f, -10f, 10f);
            offset.z = EditorGUILayout.Slider("Z", Mathf.Round(offset.z * 10f) / 10f, -10f, 10f);

            rotation.x = EditorGUILayout.Slider("X Rotation", Mathf.Round(rotation.x * 10f) / 10f, -180f, 180f);
            rotation.y = EditorGUILayout.Slider("Y Rotation", Mathf.Round(rotation.y * 10f) / 10f, -180f, 180f);
            rotation.z = EditorGUILayout.Slider("Z Rotation", Mathf.Round(rotation.z * 10f) / 10f, -180f, 180f);

            // Move and rotate the dot based on sliders
            selectedDot.transform.position = offset;
            selectedDot.transform.rotation = Quaternion.Euler(rotation);

            // Update the preview
            if (!missingFields)
            {
                DrawAnnotationPreview(offset, rotation, annotationText);
            }
        }

        if (GUILayout.Button("List All Annotations"))
        {
            ListAllAnnotationsInHierarchy();
        }
    }

    private void OnDestroy()
    {
        SaveAnnotations(); // Save annotations data when window is closed
    }

    private static void LoadAnnotations(AnnotationWindow window)
    {
        string json = EditorPrefs.GetString(AnnotationsKey);
        if (!string.IsNullOrEmpty(json))
        {
            window.annotations = JsonUtility.FromJson<AnnotationDataList>(json).annotations;
            foreach (var annotation in window.annotations)
            {
                annotation.GetTargetRenderer();
            }
        }
    }

    public void SaveAnnotations()
    {
        Debug.Log("Annotations Saved");
        foreach (var annotation in annotations)
        {
            annotation.rendererInstanceID = annotation.targetRenderer != null ? annotation.targetRenderer.GetInstanceID() : 0;
        }
        string json = JsonUtility.ToJson(new AnnotationDataList { annotations = annotations });
        EditorPrefs.SetString(AnnotationsKey, json);
    }

    private void ShowEditWindow()
    {
        AnnotationEditWindow editWindow = ScriptableObject.CreateInstance<AnnotationEditWindow>();
        editWindow.Init(this, selectedAnnotation, lineMaterial, textPrefab, targetCanvas);
        editWindow.ShowUtility();
    }

    private void AddAnnotation(AnnotationData annotationData, Vector3 offset, Vector3 rotation, Canvas canvas)
    {
        Debug.Log("Adding annotation for: " + targetRenderer.gameObject.name);
        Bounds bounds;

        if (targetRenderer is MeshRenderer)
        {
            bounds = ((MeshRenderer)targetRenderer).bounds;
        }
        else if (targetRenderer is SkinnedMeshRenderer)
        {
            bounds = ((SkinnedMeshRenderer)targetRenderer).bounds;
        }
        else
        {
            Debug.LogWarning("Unknown renderer type. Cannot add annotation.");
            return;
        }

        Vector3 annotationPosition = bounds.center + offset;

        if (textPrefab != null)
        {
            GameObject textInstance = Instantiate(textPrefab, canvas.transform);
            TextMeshProUGUI textComponent = textInstance.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                annotationData.text = textComponent;
                annotationData.text.rectTransform.position = annotationPosition;
                annotationData.text.rectTransform.rotation = Quaternion.Euler(rotation);
                annotationData.text.text = annotationText;
            }
            else
            {
                Debug.LogWarning("No TextMeshProUGUI component found on the Text Prefab.");
            }
        }

        GameObject lineObject = new GameObject("AnnotationLine");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, annotationPosition);
        lineRenderer.SetPosition(1, bounds.center);

        annotationData.line = lineRenderer;
        annotationData.targetRenderer = targetRenderer;
    }

    private void AddDotAnnotation(AnnotationData annotationData, GameObject dot, Vector3 offset, Vector3 rotation, Canvas canvas)
    {
        Vector3 annotationPosition = dot.transform.position + offset;

        if (textPrefab != null)
        {
            GameObject textInstance = Instantiate(textPrefab, canvas.transform);
            TextMeshProUGUI textComponent = textInstance.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                annotationData.text = textComponent;
                annotationData.text.rectTransform.position = annotationPosition;
                annotationData.text.rectTransform.rotation = Quaternion.Euler(rotation);
                annotationData.text.text = annotationText;
            }
            else
            {
                Debug.LogWarning("No TextMeshProUGUI component found on the Text Prefab.");
            }
        }

        GameObject lineObject = new GameObject("AnnotationLine");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, annotationPosition);
        lineRenderer.SetPosition(1, dot.transform.position);

        annotationData.line = lineRenderer;
        annotationData.targetRenderer = null; // No target renderer
        annotationData.dot = dot; // Store reference to the dot
    }

    private void RemoveAnnotation(AnnotationData annotation)
    {
        if (annotation.text != null)
        {
            DestroyImmediate(annotation.text.gameObject);
        }

        if (annotation.line != null)
        {
            DestroyImmediate(annotation.line.gameObject);
        }

        if (annotation.dot != null)
        {
            DestroyImmediate(annotation.dot); // Destroy the dot
        }

        annotations.Remove(annotation);
        SaveAnnotations();
    }

    private void ResetFields()
    {
        targetRenderer = null;
        annotationText = "";
        offset = Vector3.up * 1;
        rotation = new Vector3(0, 90, 0);
    }

    private void DestroyPreview()
    {
        if (previewText != null)
        {
            DestroyImmediate(previewText);
            previewText = null;
        }

        if (previewLine != null)
        {
            DestroyImmediate(previewLine.gameObject);
            previewLine = null;
        }
    }

    private void DrawAnnotationPreview(Vector3 offset, Vector3 rotation, string text)
    {
        Bounds bounds = new Bounds();

        if (targetRenderer != null)
        {
            if (targetRenderer is MeshRenderer)
            {
                bounds = ((MeshRenderer)targetRenderer).bounds;
            }
            else if (targetRenderer is SkinnedMeshRenderer)
            {
                bounds = ((SkinnedMeshRenderer)targetRenderer).bounds;
            }
            else
            {
                Debug.LogWarning("Unknown renderer type. Cannot draw preview.");
                return;
            }
        }

        Vector3 annotationPosition = bounds.center + offset;

        if (previewText == null && textPrefab != null)
        {
            previewText = Instantiate(textPrefab, targetCanvas.transform);
            previewText.GetComponent<TextMeshProUGUI>().text = text;
        }

        if (previewText != null)
        {
            previewText.transform.position = annotationPosition;
            previewText.transform.rotation = Quaternion.Euler(rotation);
        }

        if (previewLine == null)
        {
            GameObject lineObject = new GameObject("AnnotationPreviewLine");
            previewLine = lineObject.AddComponent<LineRenderer>();
            previewLine.material = lineMaterial;
            previewLine.startWidth = 0.01f;
            previewLine.endWidth = 0.01f;
            previewLine.positionCount = 2;
        }

        if (previewLine != null)
        {
            previewLine.SetPosition(0, annotationPosition);
            previewLine.SetPosition(1, bounds.center);
        }
    }

    private void SpawnDot()
    {
        GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dot.transform.position = Vector3.zero;
        dot.transform.localScale = Vector3.one * 0.1f;
        dot.GetComponent<Renderer>().sharedMaterial.color = Color.white; // Updated
        spawnedDots.Add(dot);
        selectedDot = dot;
    }

    public void UpdateAnnotation(AnnotationData annotation, string newText, Vector3 newPosition, Vector3 newRotation)
    {
        LineRenderer lineRenderer = annotation.text.transform.Find("Line").GetComponent<LineRenderer>();
        Bounds bounds;

        if (annotation.targetRenderer is MeshRenderer)
        {
            bounds = ((MeshRenderer)annotation.targetRenderer).bounds;
        }
        else if (annotation.targetRenderer is SkinnedMeshRenderer)
        {
            bounds = ((SkinnedMeshRenderer)annotation.targetRenderer).bounds;
        }
        else
        {
            Debug.LogWarning("Unknown renderer type. Cannot update annotation.");
            return;
        }
        // Update line renderer position
        Vector3 annotationPosition = newPosition;
        Vector3 annotationPositionWithOffset = bounds.center + annotationPosition;
        lineRenderer.SetPosition(0, annotationPositionWithOffset);
        lineRenderer.SetPosition(1, bounds.center);

        // Update text position and rotation
        annotation.text.rectTransform.position = annotationPositionWithOffset;
        annotation.text.rectTransform.rotation = Quaternion.Euler(newRotation);

        // Update annotation data
        annotation.ownText = newText;
        annotation.position = newPosition; // Update position
        annotation.rotation = newRotation; // Update rotation

        annotation.text.text = newText;
    }


    private void ListAllAnnotationsInHierarchy()
    {
        foreach (AnnotationData annotation in annotations)
        {
            if (annotation.text != null)
            {
                Debug.Log("Annotation: " + annotation.text.text + ", Position: " + annotation.text.transform.position);
            }
        }
    }
}


[Serializable]
public class AnnotationData
{
    public string ownText;
    public Renderer targetRenderer;
    public TextMeshProUGUI text;
    public LineRenderer line;
    public int rendererInstanceID;
    public GameObject dot; // Reference to the dot object
    public Vector3 position; // Add position property
    public Vector3 rotation; // Add rotation property

    public void SetTargetRenderer(Renderer renderer)
    {
        targetRenderer = renderer;
        rendererInstanceID = renderer != null ? renderer.GetInstanceID() : 0;
    }

    public void GetTargetRenderer()
    {
        if (rendererInstanceID != 0)
        {
            targetRenderer = EditorUtility.InstanceIDToObject(rendererInstanceID) as Renderer;
        }
    }
}


[Serializable]
public class AnnotationDataList
{
    public List<AnnotationData> annotations;
}
public class AnnotationEditWindow : EditorWindow
{
    private AnnotationWindow parentWindow;
    private AnnotationData annotationData;
    private Material lineMaterial;
    private GameObject textPrefab;
    private Canvas targetCanvas;

    private string annotationText;
    private Vector3 offset;
    private Vector3 rotation;

    public void Init(AnnotationWindow parent, AnnotationData data, Material lineMat, GameObject textPrefab, Canvas canvas)
    {
        parentWindow = parent;
        annotationData = data;
        lineMaterial = lineMat;
        this.textPrefab = textPrefab;
        targetCanvas = canvas;

        annotationText = data.ownText;
        offset = data.position - (data.targetRenderer != null ? data.targetRenderer.bounds.center : Vector3.zero);
        rotation = data.rotation;
    }

    private void OnGUI()
    {
        GUILayout.Label("Edit Annotation", EditorStyles.boldLabel);

        annotationText = EditorGUILayout.TextField("Annotation Text", annotationText);

        GUILayout.Label("Offset", EditorStyles.boldLabel);
        offset.x = EditorGUILayout.Slider("X", Mathf.Round(offset.x * 10f) / 10f, -10f, 10f);
        offset.y = EditorGUILayout.Slider("Y", Mathf.Round(offset.y * 10f) / 10f, -10f, 10f);
        offset.z = EditorGUILayout.Slider("Z", Mathf.Round(offset.z * 10f) / 10f, -10f, 10f);

        GUILayout.Label("Rotation", EditorStyles.boldLabel);
        rotation.x = EditorGUILayout.Slider("X", Mathf.Round(rotation.x * 10f) / 10f, -180f, 180f);
        rotation.y = EditorGUILayout.Slider("Y", Mathf.Round(rotation.y * 10f) / 10f, -180f, 180f);
        rotation.z = EditorGUILayout.Slider("Z", Mathf.Round(rotation.z * 10f) / 10f, -180f, 180f);

        if (GUILayout.Button("Save"))
        {
            if (annotationData.targetRenderer is MeshRenderer || annotationData.targetRenderer is SkinnedMeshRenderer)
            {
                parentWindow.UpdateAnnotation(annotationData, annotationText, offset, rotation);
                parentWindow.SaveAnnotations();
                this.Close();
            }
            else
            {
                Debug.LogWarning("Target Renderer must be either a MeshRenderer or a SkinnedMeshRenderer.");
            }
        }

        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }
    }
}
*/