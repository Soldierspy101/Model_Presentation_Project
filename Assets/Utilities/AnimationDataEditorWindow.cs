/*
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AnimationDataEditorWindow : EditorWindow
{
    private AnimationManager animationManager;
    private SerializedObject serializedObject;
    private SerializedProperty animationsProp;
    private Vector2 scrollPosition;
    private int selectedModelIndex = 0;
    private string[] modelNames;
    private GameObject[] modelObjects;

    [MenuItem("Tools/Animation Manager")]
    public static void ShowWindow()
    {
        GetWindow<AnimationDataEditorWindow>("Animation Manager");
    }

    private void OnEnable()
    {
        animationManager = FindObjectOfType<AnimationManager>();
        if (animationManager != null)
        {
            serializedObject = new SerializedObject(animationManager);
            animationsProp = serializedObject.FindProperty("animations");
        }
    }

    private void OnGUI()
    {
        if (animationManager == null)
        {
            EditorGUILayout.HelpBox("AnimationManager script not found in the scene.", MessageType.Warning);
            return;
        }

        serializedObject.Update();

        GUILayout.Label("Manage Animations", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Animation"))
        {
            animationsProp.arraySize++;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(180));

        for (int i = 0; i < animationsProp.arraySize; i++)
        {
            SerializedProperty animationData = animationsProp.GetArrayElementAtIndex(i);
            SerializedProperty animationClipProp = animationData.FindPropertyRelative("animationClip");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(animationClipProp);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                animationsProp.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties(); // Apply modification after deletion
                break; // Exit the loop after removing the animation
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        // Dropdown to select model
        GameObject addModelHere = GameObject.Find("AddModelHere");
        if (addModelHere == null)
        {
            EditorGUILayout.HelpBox("AddModelHere GameObject not found.", MessageType.Warning);
            return;
        }

        // Filter out the "TV" object and collect model objects
        int validModelCount = 0;
        for (int i = 0; i < addModelHere.transform.childCount; i++)
        {
            if (addModelHere.transform.GetChild(i).name != "TV")
            {
                validModelCount++;
            }
        }

        modelObjects = new GameObject[validModelCount];
        modelNames = new string[validModelCount];
        int count = 0;

        for (int i = 0; i < addModelHere.transform.childCount; i++)
        {
            if (addModelHere.transform.GetChild(i).name != "TV")
            {
                modelNames[count] = addModelHere.transform.GetChild(i).name;
                modelObjects[count] = addModelHere.transform.GetChild(i).gameObject;
                count++;
            }
        }

        if (validModelCount > 0)
        {
            selectedModelIndex = EditorGUILayout.Popup("Select Model", selectedModelIndex, modelNames);

            GameObject selectedModel = modelObjects[selectedModelIndex];

            EditorGUILayout.LabelField("Animations for " + selectedModel.name, EditorStyles.boldLabel);

            if (GUILayout.Button("Apply Selected Animations to Model"))
            {
                SerializedProperty animationClipProp = animationsProp.GetArrayElementAtIndex(selectedModelIndex).FindPropertyRelative("animationClip");

                if (animationClipProp.objectReferenceValue != null)
                {
                    CustomAnimation newAnimation = new CustomAnimation();
                    newAnimation.animationClip = animationClipProp.objectReferenceValue as AnimationClip;
                    newAnimation.modelName = selectedModel.name;
                    animationManager.ApplyAnimation(newAnimation, selectedModel);
                }
                else
                {
                    EditorUtility.DisplayDialog("No Animation Selected", "Please add an animation before applying it to the model.", "OK");
                }
            }

            // Display the animations and animator components added to the selected model
            DisplayAndRemoveAnimations(selectedModel);
        }
        else
        {
            EditorGUILayout.HelpBox("No models found in 'AddModelHere'.", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayAndRemoveAnimations(GameObject selectedModel)
    {
        Animation anim = selectedModel.GetComponent<Animation>();
        if (anim != null)
        {
            GUILayout.Label("Animation Clips on " + selectedModel.name, EditorStyles.boldLabel);
            foreach (AnimationState animState in anim)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(animState.name);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    anim.RemoveClip(animState.name);
                    break; // Exit the loop after removing the animation
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        Animator animator = selectedModel.GetComponent<Animator>();
        if (animator != null)
        {
            GUILayout.Label("Animator Controller on " + selectedModel.name, EditorStyles.boldLabel);
            if (GUILayout.Button("Remove Animator"))
            {
                DestroyImmediate(animator);
            }
        }
    }
}
*/