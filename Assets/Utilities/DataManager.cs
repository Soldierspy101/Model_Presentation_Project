using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public TitleDescriptionData data = new TitleDescriptionData();

    // Save data to a JSON file
    public void SaveData(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            Debug.LogError("File path is not set.");
            return;
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filename, json);
    }


    // Load data from a JSON file
    public void LoadData(string filename)
    {
        if (File.Exists(filename))
        {
            string json = File.ReadAllText(filename);
            data = JsonUtility.FromJson<TitleDescriptionData>(json);
            Debug.Log("Data loaded: " + json);
        }
        else
        {
            Debug.LogWarning("Data file not found.");
        }
    }

    // Reset data
    public void ResetData(string filename)
    {
        data.title = "";
        data.description = "";
        SaveData(filename);
    }
}

[System.Serializable]
public class TitleDescriptionData
{
    public string title;
    public string description;
}
