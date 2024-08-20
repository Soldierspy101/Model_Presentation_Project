using UnityEngine;

public class RotateModel1 : MonoBehaviour
{
    public void RotateUp()
    {
        transform.Rotate(Vector3.right * 90);
    }

    public void RotateDown()
    {
        transform.Rotate(Vector3.left * 90);
    }

    public void RotateLeft()
    {
        transform.Rotate(Vector3.down * 90);
    }

    public void RotateRight()
    {
        transform.Rotate(Vector3.up * 90);
    }
}