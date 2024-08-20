using UnityEngine;

public class PlefReset1 : MonoBehaviour
{
    // Method to reset all PlayerPrefs
    public void ResetAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Ensure the changes are saved to disk immediately
        Debug.Log("All PlayerPrefs have been reset.");
    }
}
