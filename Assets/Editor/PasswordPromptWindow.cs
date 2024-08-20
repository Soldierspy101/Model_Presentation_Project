using UnityEngine;
using UnityEditor;

public class PasswordPromptWindow : EditorWindow
{
    private const string correctPassword = "1234567";
    private string enteredPassword = "";
    private static System.Action<bool> onPasswordValidated;

    public static void ShowPasswordPrompt(System.Action<bool> callback)
    {
        onPasswordValidated = callback;
        PasswordPromptWindow window = ScriptableObject.CreateInstance<PasswordPromptWindow>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 100);
        window.ShowPopup();
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter Password", EditorStyles.boldLabel);
        enteredPassword = EditorGUILayout.PasswordField("Password", enteredPassword);

        if (GUILayout.Button("Submit"))
        {
            if (enteredPassword == correctPassword)
            {
                onPasswordValidated?.Invoke(true);
                this.Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Incorrect password!", "OK");
                onPasswordValidated?.Invoke(false);
            }
        }

        if (GUILayout.Button("Cancel"))
        {
            onPasswordValidated?.Invoke(false);
            this.Close();
        }
    }
}
