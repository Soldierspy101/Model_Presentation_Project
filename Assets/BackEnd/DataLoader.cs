using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class DataLoader : MonoBehaviour
{
    public static DataLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadData<T>(string fileName, System.Action<T> callback) where T : class
    {
        StartCoroutine(LoadDataCoroutine(fileName, callback));
    }

    private IEnumerator LoadDataCoroutine<T>(string fileName, System.Action<T> callback) where T : class
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL uses UnityWebRequest to load files
            using (UnityWebRequest www = UnityWebRequest.Get(path))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    T data = JsonUtility.FromJson<T>(www.downloadHandler.text);
                    callback?.Invoke(data);
                }
                else
                {
                    Debug.LogError($"Error loading data: {www.error}");
                }
            }
        }
        else
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                T data = JsonUtility.FromJson<T>(json);
                callback?.Invoke(data);
            }
            else
            {
                Debug.LogError("File not found: " + path);
            }
        }
    }

}
