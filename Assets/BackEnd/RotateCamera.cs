using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public Transform target; // Reference to the object you want to rotate around
    public float rotationSpeed = 90f; // Rotation speed in degrees per second

    public void RotateTopView()
    {
        // Align camera with top view of the model
        transform.position = target.position + Vector3.up * 2f; // Adjust position to be above the model
        transform.LookAt(target); // Look at the model
    }

    public void RotateBottomView()
    {
        // Align camera with front view of the model
        transform.position = target.position + Vector3.back * 2f; // Adjust position to be behind the model
        transform.LookAt(target); // Look at the model
    }

    public void RotateLeftView()
    {
        // Align camera with left view of the model
        transform.position = target.position + Vector3.left * 2f; // Adjust position to the left of the model
        transform.LookAt(target); // Look at the model
    }

    public void RotateRightView()
    {
        // Align camera with right view of the model
        transform.position = target.position + Vector3.right * 2f; // Adjust position to the right of the model
        transform.LookAt(target); // Look at the model
    }

    // Example method for rotating to a custom view
    public void RotateCustomView(Vector3 positionOffset, Vector3 targetOffset)
    {
        // Adjust position and look at the model with specified offsets
        transform.position = target.position + positionOffset;
        transform.LookAt(target.position + targetOffset);
    }
}
