using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FileAccessTest : MonoBehaviour
{
    public string fileName = "TitleDescriptionData.json";

    void Start()
    {
        // Use the StreamingAssets path
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        Debug.Log("StreamingAssets Path: " + path);

        StartCoroutine(CheckFileAccess(path));
    }

    IEnumerator CheckFileAccess(string path)
    {
        // For WebGL, use UnityWebRequest to access the file
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("File content: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}
