using System.Collections.Generic;
using UnityEngine;

public static class Constants1
{
    public const string HIDDEN_FLAG = "HiddenFlag";
    public static readonly HashSet<string> HiddenObjectNames = new HashSet<string>
    {
        "Directional Light",
        "Enviroment",
        "Camera",
        "EventSystem",
        "UI",
        "DragOptionsHere",
        "3D"
    };
}
public class Hidden1 : MonoBehaviour
{
    private void OnValidate()
    {
        RefreshHiddenState();
    }

    public void RefreshHiddenState()
    {
#if UNITY_EDITOR
        bool hidden = UnityEditor.EditorPrefs.GetBool(Constants1.HIDDEN_FLAG, false);
        if (Constants1.HiddenObjectNames.Contains(this.gameObject.name))
        {
            this.gameObject.hideFlags = hidden ? HideFlags.HideInHierarchy : HideFlags.None;
        }
        else
        {
            this.gameObject.hideFlags = HideFlags.None;
        }
        UnityEditor.EditorApplication.RepaintHierarchyWindow(); // Refresh the hierarchy window
#endif
    }
}