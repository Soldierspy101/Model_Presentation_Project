/*
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class TextEditorWindow : EditorWindow
{
    private string newTitle = "";
    private string newDescription = "";
    private Text titleTextObject;
    private Text descriptionTextObject;
    private const string TitlePlayerPrefsKey = "TextEditorWindow_Title";
    private const string DescriptionPlayerPrefsKey = "TextEditorWindow_Description";

    [MenuItem("Tools/Text Editor")]
    public static void ShowWindow()
    {
        TextEditorWindow window = GetWindow<TextEditorWindow>("Text Editor");
        window.titleTextObject = GameObject.Find("Title")?.GetComponent<Text>();
        window.descriptionTextObject = GameObject.Find("Description")?.GetComponent<Text>();

        if (window.titleTextObject == null)
        {
            Debug.LogWarning("Title object not found in the scene.");
        }

        if (window.descriptionTextObject == null)
        {
            Debug.LogWarning("Description object not found in the scene.");
        }

        // Load saved text from PlayerPrefs
        window.LoadSavedText();
    }

    private void LoadSavedText()
    {
        newTitle = PlayerPrefs.GetString(TitlePlayerPrefsKey, "");
        newDescription = PlayerPrefs.GetString(DescriptionPlayerPrefsKey, "");

        if (titleTextObject != null && descriptionTextObject != null)
        {
            titleTextObject.text = newTitle;
            descriptionTextObject.text = newDescription;
        }
    }

    private void OnGUI()
    {
        // Increase the height of the text field
        GUILayout.Label("Edit Title", EditorStyles.boldLabel);
        newTitle = EditorGUILayout.TextField("New Title", newTitle);

        // Use TextArea for multiline text input
        GUILayout.Label("Edit Description", EditorStyles.boldLabel);
        newDescription = EditorGUILayout.TextArea(newDescription, GUILayout.Height(100)); // Adjust the height as needed

        // Apply changes in real-time
        ApplyChanges();

        if (GUILayout.Button("Apply"))
        {
            ApplyChanges();
        }
    }

    private void ApplyChanges()
    {
        if (titleTextObject != null && descriptionTextObject != null)
        {
            if (string.IsNullOrEmpty(newTitle.Trim()) || string.IsNullOrEmpty(newDescription.Trim()))
            {
                Debug.LogWarning("New title or description is empty.");
                return;
            }

            // Save the new text to PlayerPrefs
            PlayerPrefs.SetString(TitlePlayerPrefsKey, newTitle);
            PlayerPrefs.SetString(DescriptionPlayerPrefsKey, newDescription);

            // Save PlayerPrefs immediately
            PlayerPrefs.Save();

            // Apply the changes to the Text objects
            titleTextObject.text = newTitle;
            descriptionTextObject.text = newDescription;
        }
        else
        {
            Debug.LogWarning("Title or Description Text Object not selected.");
        }
    }

}
*/