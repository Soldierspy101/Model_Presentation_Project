using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class HiddenMenuItem1
{
    const string k_menu = "Game/Show Hidden Objects";

    [MenuItem(k_menu)]
    static void ShowHiddenMenuItem()
    {
        bool currentHiddenState = !EditorPrefs.GetBool(Constants1.HIDDEN_FLAG, false);

        if (!currentHiddenState) // Only ask for password when unhiding
        {
            PasswordPromptWindow.ShowPasswordPrompt((isPasswordValid) =>
            {
                if (isPasswordValid)
                {
                    ToggleHiddenState(currentHiddenState);
                }
            });
        }
        else
        {
            ToggleHiddenState(currentHiddenState);
        }
    }

    private static void ToggleHiddenState(bool currentHiddenState)
    {
        EditorPrefs.SetBool(Constants1.HIDDEN_FLAG, currentHiddenState); // Store the updated value
        foreach (Hidden1 hidden in GameObject.FindObjectsOfType<Hidden1>())
        {
            hidden.RefreshHiddenState();
        }
    }

    [MenuItem(k_menu, true)]
    static bool ShowHiddenMenuItemValidation()
    {
        Menu.SetChecked(k_menu, !EditorPrefs.GetBool(Constants1.HIDDEN_FLAG, false));
        return true;
    }
}
