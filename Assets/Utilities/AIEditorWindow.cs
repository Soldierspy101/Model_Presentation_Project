/*
using UnityEditor;
using UnityEngine;

public class AIEditorWindow : EditorWindow
{    
    private OpenAIController openAIController;
    private string additionalSystemInfo = "";
    private Vector2 scrollPosition;

    [MenuItem("Tools/AI Editor")]
    public static void ShowWindow()
    {
        GetWindow<AIEditorWindow>("AI Editor");
    }

    private void OnEnable()
    {
        openAIController = Utils.FindObjectOfType<OpenAIController>(true);
        if (openAIController == null)
        {
            Debug.LogError("OpenAI Controller not found in the scene.");
        }
        else
        {
            // Load additionalInfo from PlayerPrefs
            additionalSystemInfo = PlayerPrefs.GetString("AdditionalInfo", "");
        }
    }

    private void OnGUI()
    {
        if (openAIController == null)
        {
            GUILayout.Label("OpenAI Controller not found in the scene.", EditorStyles.boldLabel);
            return;
        }

        GUILayout.Label("Additional Product Info", EditorStyles.boldLabel);
        
        // Create a style with word wrap enabled
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea)
        {
            wordWrap = true
        };

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100)); // Adjust the height as needed
        additionalSystemInfo = EditorGUILayout.TextArea(additionalSystemInfo, textAreaStyle, GUILayout.ExpandHeight(true)); // Apply the custom style
        EditorGUILayout.EndScrollView(); // End scroll view

        if (GUILayout.Button("Update AI Chatbot"))
        {
            UpdateSystemMessage();
        }
    }

    private void UpdateSystemMessage()
    {
        if (openAIController != null)
        {
            Debug.Log("Updating AI Chatbot.");
            openAIController.SetAdditionalInfo(additionalSystemInfo);
        }
        else
        {
            Debug.LogError("OpenAIController is null. Cannot update AI Chatbot.");
        }
    }

    public static class Utils
    {
        public static T FindObjectOfType<T>(bool includeInactive) where T : Object
        {
            T[] objects = Resources.FindObjectsOfTypeAll<T>();
            foreach (T obj in objects)
            {
                if (obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave)
                    continue;

                if (includeInactive || ((obj as Component)?.gameObject.activeInHierarchy ?? true))
                    return obj;
            }
            return null;
        }
    }
}
*/