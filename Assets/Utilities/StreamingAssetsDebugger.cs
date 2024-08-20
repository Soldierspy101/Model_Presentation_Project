using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class StreamingAssetsDebugger : MonoBehaviour
{
    void Start()
    {
        // Log the path to StreamingAssets for debugging
        Debug.Log("StreamingAssets Path: " + Application.streamingAssetsPath);

        // Start the coroutine to check file access
        StartCoroutine(CheckFileAccess());
    }

    private IEnumerator CheckFileAccess()
    {
        // Construct the file path for WebGL and other platforms
        string filePath = Path.Combine(Application.streamingAssetsPath, "TitleDescriptionData.json");

        // Check if running on WebGL
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // Use UnityWebRequest to get the file from StreamingAssets
            using (UnityWebRequest www = UnityWebRequest.Get(filePath))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    // Log success and file content
                    Debug.Log("File loaded successfully.");
                    Debug.Log("File content:\n" + www.downloadHandler.text);
                }
                else
                {
                    // Log errors if file cannot be loaded
                    Debug.LogError($"Error loading file: {www.error}");
                    Debug.LogError($"Response Code: {www.responseCode}");
                }
            }
        }
        else
        {
            // Check file existence for other platforms (Editor, Standalone)
            if (File.Exists(filePath))
            {
                // Log success and file content
                Debug.Log("File exists.");
                string content = File.ReadAllText(filePath);
                Debug.Log("File content:\n" + content);
            }
            else
            {
                // Log errors if file is not found
                Debug.LogError("File not found: " + filePath);
            }
        }
    }
}
