using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveAnnotation1 : MonoBehaviour
{
    public Transform targetTransform;
    private GameObject previousModel;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Called when a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if there's a previous model GameObject
        if (previousModel != null)
        {
            // Destroy the previous model
            Destroy(previousModel);
        }

        // Move the object to the target position
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;

        // Set the reference to the current model GameObject
        previousModel = gameObject;
    }

    // Unsubscribe from the event when the object is destroyed
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
