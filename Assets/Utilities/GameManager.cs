using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text titleTextObject;
    public Text descriptionTextObject;
    [Header("JSON Files")]
    public string titleDescriptionDataFileName = "TitleDescriptionData.json";

    private void Start()
    {
        LoadTitleDescriptionData();
    }

    private void LoadTitleDescriptionData()
    {
        DataLoader.Instance.LoadData<TitleDescriptionData>(titleDescriptionDataFileName, OnTitleDescriptionDataLoaded);
    }

    private void OnTitleDescriptionDataLoaded(TitleDescriptionData data)
    {
        if (titleTextObject != null && descriptionTextObject != null)
        {
            // Handle the loaded TitleDescriptionData
            Debug.Log("Title: " + data.title);
            Debug.Log("Description: " + data.description);

            // Apply the loaded data to UI elements
            titleTextObject.text = data.title;
            descriptionTextObject.text = data.description;
        }
        else
        {
            Debug.LogWarning("UI components are not assigned.");
        }
    }
}
