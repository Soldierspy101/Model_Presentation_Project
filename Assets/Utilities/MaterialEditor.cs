/*
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using System.Linq;

public class MaterialEditor : EditorWindow
{
    private GameObject dragOptionsGameObject;
    private List<ChangeMaterial1> changeMaterialScripts = new List<ChangeMaterial1>();
    private Dictionary<ChangeMaterial1, bool> scriptFoldouts = new Dictionary<ChangeMaterial1, bool>();
    private Renderer targetRenderer;
    private List<Material> materials = new List<Material>();
    private Button changeMaterialButton; // The button to change the material

    private bool isFourthFunctionEnabled = false;

    private void OnEnable()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;

        if (!Application.isPlaying)
        {
            ShowWindow();
        }
    }


    private void OnDisable()
    {
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
    }

    private void OnHierarchyChanged()
    {
        GameObject fourthFunction = GameObject.Find("FourthFunction");
        isFourthFunctionEnabled = fourthFunction != null && fourthFunction.activeInHierarchy;
        Repaint(); // Repaint the window to update the GUI
    }

    private void OnGUI()
    {
        bool missingFields = changeMaterialScripts.Any(script =>
        script.targetRenderer == null || script.materials == null || script.materials.Length == 0);

        if (missingFields)
        {
            EditorGUILayout.HelpBox("Please fill in all required fields.", MessageType.Warning);
        }

        GUILayout.Label("Change Material Settings", EditorStyles.boldLabel);

        if (dragOptionsGameObject == null)
        {
            dragOptionsGameObject = GameObject.Find("DragOptionsHere");
        }

        dragOptionsGameObject = EditorGUILayout.ObjectField("Drag Options GameObject", dragOptionsGameObject, typeof(GameObject), true) as GameObject;

        if (dragOptionsGameObject != null)
        {
            // Get all ChangeMaterial1 scripts from the DragOptionsHere GameObject
            changeMaterialScripts.Clear();
            ChangeMaterial1[] allChangeMaterial1Scripts = dragOptionsGameObject.GetComponentsInChildren<ChangeMaterial1>(true);
            changeMaterialScripts.AddRange(allChangeMaterial1Scripts);

            EditorGUILayout.Space();

            GUILayout.Label("Change Material Scripts", EditorStyles.boldLabel);

            foreach (var script in changeMaterialScripts.ToList())
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                try
                {
                    EditorGUILayout.BeginHorizontal();
                    try
                    {
                        GUILayout.Label("ChangeMaterial1", GUILayout.Width(100));
                        if (GUILayout.Button("Remove", GUILayout.Width(70)))
                        {
                            scriptFoldouts.Remove(script); // Remove the corresponding foldout state
                            DestroyImmediate(script);
                            changeMaterialScripts.Remove(script);
                            break;
                        }
                        if (GUILayout.Button("Edit", GUILayout.Width(70)))
                        {
                            ToggleFoldout(script);
                        }
                    }
                    finally
                    {
                        EditorGUILayout.EndHorizontal();
                    }

                    if (scriptFoldouts.TryGetValue(script, out bool isFoldout) && isFoldout)
                    {
                        EditorGUI.indentLevel++;
                        RenderMaterialScript(script);
                        EditorGUI.indentLevel--;
                    }
                }
                finally
                {
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add ChangeMaterial1 Script"))
            {
                ChangeMaterial1 newChangeMaterial1 = dragOptionsGameObject.AddComponent<ChangeMaterial1>();
                changeMaterialScripts.Add(newChangeMaterial1);
                scriptFoldouts[newChangeMaterial1] = false; // Initialize the foldout state for the new script

                // Auto fill the changeMaterialButton field
                newChangeMaterial1.changeMaterialButton = changeMaterialButton;
            }


            EditorGUILayout.Space();

            if (changeMaterialButton == null)
            {
                GameObject uiGameObject = GameObject.Find("Functions");
                if (uiGameObject != null)
                {
                    GameObject FourthFunc = GameObject.Find("FourthFunction");
                    if (FourthFunc != null)
                    {
                        changeMaterialButton = FourthFunc.GetComponentInChildren<Button>();
                        if (changeMaterialButton != null)
                        {
                            foreach (var script in changeMaterialScripts)
                            {
                                script.changeMaterialButton = changeMaterialButton;
                            }
                        }
                    }
                }
            }
        }
    }

    private void ToggleFoldout(ChangeMaterial1 script)
    {
        if (scriptFoldouts.ContainsKey(script))
        {
            scriptFoldouts[script] = !scriptFoldouts[script];
        }
        else
        {
            scriptFoldouts.Add(script, true);
        }
    }

    private void RenderMaterialScript(ChangeMaterial1 script)
    {
        if (script == null)
        {
            return;
        }

        // Display renderer and material options based on the selected script
        script.targetRenderer = EditorGUILayout.ObjectField("Target Renderer", script.targetRenderer, typeof(Renderer), true) as Renderer;

        int newMaterialsCount = script.materials != null ? script.materials.Length : 0;
        newMaterialsCount = EditorGUILayout.IntField("Materials Count", newMaterialsCount);

        if (newMaterialsCount < 0)
        {
            newMaterialsCount = 0;
        }

        List<Material> newMaterials = new List<Material>();
        if (script.materials != null)
        {
            newMaterials.AddRange(script.materials);
        }

        // Update the materials array of the script
        while (newMaterialsCount < newMaterials.Count)
        {
            newMaterials.RemoveAt(newMaterials.Count - 1);
        }
        while (newMaterialsCount > newMaterials.Count)
        {
            newMaterials.Add(null);
        }

        // Allow selecting materials
        for (int i = 0; i < newMaterials.Count; i++)
        {
            newMaterials[i] = EditorGUILayout.ObjectField("Material " + (i + 1), newMaterials[i], typeof(Material), true) as Material;
        }

        script.materials = newMaterials.ToArray();
    }


    [MenuItem("Tools/Change Material Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MaterialEditor));
    }
}
*/