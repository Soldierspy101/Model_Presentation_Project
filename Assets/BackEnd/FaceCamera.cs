using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void Update()
    {
        // Get the main camera
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            // Make the annotation face the camera
            transform.LookAt(mainCamera.transform);

            // Rotate 180 degrees to correct the orientation
            transform.Rotate(0, 180, 0);
        }
    }
}
