using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnnotationScript1 : MonoBehaviour
{
    [System.Serializable]
    public class AnnotationData
    {
        public TextMeshProUGUI text;
        public string ownText;
    }

    [System.Serializable]
    public class PartData
    {
        public Renderer targetRenderer; // Change to Renderer type to accept both MeshRenderer and SkinnedMeshRenderer
        public AnnotationData annotationData;
        public Vector3 offset = Vector3.up ; // Offset property
        public Vector3 rotation; // Rotation property
    }

    [SerializeField] private PartData[] AnnotationpartDataArray;

    [SerializeField] private Material lineMaterial; // Material for the line renderer

    void Start()
    {
        Debug.Log("Start method called.");
        // Loop through each annotation part data
        foreach (PartData partData in AnnotationpartDataArray)
        {
            AddAnnotation(partData.targetRenderer, partData.annotationData, partData.offset, partData.rotation); // Pass offset and rotation
        }
    }

    void AddAnnotation(Renderer targetRenderer, AnnotationData annotationData, Vector3 offset, Vector3 rotation)
    {
        Debug.Log("Adding annotation for: " + targetRenderer.gameObject.name);
        // Check if the targetRenderer is null
        if (targetRenderer == null)
        {
            Debug.LogError("Target Renderer is null for: " + annotationData.text.gameObject.name);
            return;
        }

        // Get the bounds of the target renderer
        Bounds bounds = targetRenderer.bounds;

        // Calculate the position for the annotation text above the renderer
        Vector3 annotationPosition = bounds.center + offset; // Apply offset

        // Instantiate the text object
        TextMeshProUGUI annotationText = Instantiate(annotationData.text, annotationPosition, Quaternion.Euler(rotation), annotationData.text.transform.parent);
        if (annotationText == null)
        {
            Debug.LogError("TextMeshProUGUI prefab not assigned!"); // Check if the prefab is assigned correctly
            return;
        }
        // Set the text of the annotation
        annotationText.text = annotationData.ownText;
        Debug.Log("Annotation text set to: " + annotationData.ownText); // Check if the text is being set correctly

        // Instantiate a line renderer as a child of the text object
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(annotationText.transform, false); // Set as child
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial; // Assign the material
        lineRenderer.startWidth = 0.02f; // Adjust width as needed
        lineRenderer.endWidth = 0.02f;

        // Set positions for the line renderer
        lineRenderer.SetPosition(0, bounds.center);
        lineRenderer.SetPosition(1, annotationPosition);
    }
}