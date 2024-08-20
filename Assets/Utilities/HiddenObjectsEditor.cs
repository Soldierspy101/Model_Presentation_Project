/*using UnityEngine;
using UnityEditor;

public class HiddenObjectsEditor : EditorWindow
{
    private const string password = "1234567";
    private bool isHidden;
    private string enteredPassword = "";

    [MenuItem("Game/Hidden Objects Editor")]
    public static void ShowWindow()
    {
        GetWindow<HiddenObjectsEditor>("Hidden Objects Editor");
    }

    private void OnGUI()
    {
        bool previousHiddenState = isHidden;
        isHidden = EditorGUILayout.Toggle("Hide Objects", isHidden);

        if (previousHiddenState != isHidden)
        {
            if (!isHidden)
            {
                enteredPassword = EditorGUILayout.PasswordField("Enter Password", enteredPassword);

                if (GUILayout.Button("Submit"))
                {
                    if (enteredPassword != password)
                    {
                        EditorUtility.DisplayDialog("Error", "Incorrect password!", "OK");
                        isHidden = true; // Reset the checkbox to checked
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Success", "Password accepted.", "OK");
                        UnityEditor.EditorPrefs.SetBool(Constants1.HIDDEN_FLAG, isHidden);
                        RefreshAllHiddenStates();
                    }
                }
                return; // Skip the remaining GUI elements until the password is correctly entered
            }

            UnityEditor.EditorPrefs.SetBool(Constants1.HIDDEN_FLAG, isHidden);
            RefreshAllHiddenStates();
        }
    }

    private void RefreshAllHiddenStates()
    {
        Hidden1[] hiddenObjects = FindObjectsOfType<Hidden1>();
        foreach (Hidden1 hidden in hiddenObjects)
        {
            hidden.RefreshHiddenState();
        }
    }
}
*/