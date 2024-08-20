using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;
using System;

public class FBXImporter : MonoBehaviour
{
    private const string animatorPath = "Assets/Resources/default.controller"; // Update this path to your Animator Controller

    public static void ImportAndSetupFBX(string fbxPath)
    {
        List<string> animationNamePrefixes = new List<string> { "Disassemble", "Scene" };
        List<AnimationClip> animationClips = new List<AnimationClip>();

        if (string.IsNullOrEmpty(fbxPath))
        {
            Debug.LogError("No FBX file selected.");
            return;
        }

        // Generate a relative path in the Assets folder
        string fileName = Path.GetFileName(fbxPath);
        string destinationPath = Path.Combine(Application.dataPath, fileName);
        string assetPath = "Assets/" + fileName;

        // Copy the FBX file to the Assets folder
        File.Copy(fbxPath, destinationPath, true);

        // Refresh the AssetDatabase to detect the new file
        AssetDatabase.Refresh();

        // Import the FBX model
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.Default);

        // Load the imported FBX asset
        GameObject fbxModel = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

        if (fbxModel == null)
        {
            Debug.LogError("FBX model not found at path: " + assetPath);
            return;
        }

        // Instantiate the model in the current scene
        GameObject instance = Instantiate(fbxModel);

        // Find or create the AddModelHere GameObject in the scene
        GameObject AddModelHere = GameObject.Find("AddModelHere");

        // Set the parent of the instantiated model to AddModelHere
        instance.transform.SetParent(AddModelHere.transform);

        // Find the existing Animator Controller
        AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorPath);

        if (animatorController == null)
        {
            Debug.LogError("Animator Controller not found at path: " + animatorPath);
            return;
        }

        // Add Animator component to the instantiated model and assign the Animator Controller
        Animator animator = instance.AddComponent<Animator>();
        animator.applyRootMotion = true;
        animator.runtimeAnimatorController = animatorController;

        // Replace animation clips in the Animator Controller with those from the FBX model
        ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        if (modelImporter == null)
        {
            Debug.LogError("ModelImporter not found for the path: " + assetPath);
            return;
        }

        // Ensure animation import settings are correct
        modelImporter.animationType = ModelImporterAnimationType.Generic;
        modelImporter.importAnimation = true; // Ensure animations are imported
        modelImporter.animationCompression = ModelImporterAnimationCompression.Off;
        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
        animatorOverrideController.runtimeAnimatorController = animator.runtimeAnimatorController;

        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate); // Force reimport to ensure settings are applied
        var assetRepresentationsAtPath = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
        foreach (var assetRepresentation in assetRepresentationsAtPath)
        {
            var animationClip = assetRepresentation as AnimationClip;

            if (animationClip != null)
            {
                bool containsKeyword = animationNamePrefixes.Any(animationNamePrefix => animationClip.name.IndexOf(animationNamePrefix, StringComparison.OrdinalIgnoreCase) >= 0);
                if (containsKeyword)
                {
                    animationClips.Add(animationClip);
                }
            }
        }

        foreach (var layer in animatorController.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                // Skip any state called "Other"
                if (state.state.name == "Other")
                {
                    continue;
                }

                // Modify the states that are not called "Other"
                state.state.motion = new BlendTree();
                BlendTree blendTree = state.state.motion as BlendTree;
                blendTree.useAutomaticThresholds = false;
                // Add new animation clips to the Blend Tree
                float threshold = 0.0f;
                for (int i = 0; i < animationClips.Count; i++)
                {
                    blendTree.AddChild(animationClips[i], threshold);
                }
            }
            // Save the changes
            AssetDatabase.SaveAssets();
            break;
        }
        animator.runtimeAnimatorController = animatorOverrideController;
    }
}
