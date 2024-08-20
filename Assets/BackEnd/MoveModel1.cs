using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveModel1 : MonoBehaviour
{
    // Define a class to hold model and target transform information
    private class ModelInfo
    {
        public GameObject model;
        public Transform targetTransform;

        public ModelInfo(GameObject model, Transform targetTransform)
        {
            this.model = model;
            this.targetTransform = targetTransform;
        }
    }

    // List to hold model information
    private static List<ModelInfo> modelInfos = new List<ModelInfo>();

    // Public variable for the target transform
    public Transform targetTransform;

    // AddModel method to add a new model to the list
    public static void AddModel(GameObject model, Transform targetTransform)
    {
        modelInfos.Add(new ModelInfo(model, targetTransform));
    }

    // RemoveModel method to remove a model from the list
    public static void RemoveModel(GameObject model)
    {
        modelInfos.RemoveAll(info => info.model == model);
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Add the current model to the list
        AddModel(gameObject, targetTransform);
    }

    // Called when a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Move each model to its target position
        foreach (var info in modelInfos)
        {
            info.model.transform.position = info.targetTransform.position;
            info.model.transform.rotation = info.targetTransform.rotation;
        }
    }

    // Unsubscribe from the event when the object is destroyed
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Remove the current model from the list
        RemoveModel(gameObject);
    }
}
