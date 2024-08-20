using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesignNavigation : MonoBehaviour
{
    private List<GameObject> imageObjects = new List<GameObject>();
    private int currentIndex = 0;
    private GameObject placeholder;

    private void Start()
    {
        // Locate the Placeholder GameObject
        placeholder = FindGameObject("UI/Functions/FourthFunction/Image/Placeholder");

        if (placeholder != null)
        {
            // Populate the list of image objects (children of the placeholder)
            foreach (Transform child in placeholder.transform)
            {
                imageObjects.Add(child.gameObject);
            }

            if (imageObjects.Count > 0)
            {
                // Initially hide all image objects
                foreach (var imageObject in imageObjects)
                {
                    imageObject.SetActive(false);
                }
                // Show the first image
                imageObjects[currentIndex].SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("Placeholder GameObject not found.");
        }
    }

    public void ToggleNextGameObject()
    {
        ToggleGameObject(1);
    }

    public void ToggleBackGameObject()
    {
        ToggleGameObject(-1);
    }

    private void ToggleGameObject(int direction)
    {
        if (imageObjects.Count == 0)
        {
            Debug.LogWarning("No image objects found.");
            return;
        }

        // Hide the current object
        imageObjects[currentIndex].SetActive(false);

        // Update index based on direction and loop around if necessary
        currentIndex = (currentIndex + direction + imageObjects.Count) % imageObjects.Count;

        // Show the next or previous object
        imageObjects[currentIndex].SetActive(true);

        // Assign material if the current object has an Image component
        Image currentImage = imageObjects[currentIndex].GetComponent<Image>();
        if (currentImage != null && placeholder != null)
        {
            Image placeholderImage = placeholder.GetComponent<Image>();
            if (placeholderImage != null)
            {
                placeholderImage.material = currentImage.material;
            }
        }
    }

    private GameObject FindGameObject(string path)
    {
        string[] parts = path.Split('/');
        GameObject currentObject = null;

        foreach (string part in parts)
        {
            if (currentObject == null)
            {
                currentObject = GameObject.Find(part);
            }
            else
            {
                Transform childTransform = currentObject.transform.Find(part);
                if (childTransform != null)
                {
                    currentObject = childTransform.gameObject;
                }
                else
                {
                    currentObject = null;
                    break;
                }
            }
        }

        return currentObject;
    }
}
 