using UnityEditor;
using UnityEngine;

public class ColorPickerWindow : EditorWindow
{
    private ColorPickerControl colorPickerControl;

    [MenuItem("Window/Color Picker Window")]
    public static void ShowWindow()
    {
        GetWindow<ColorPickerWindow>("Color Picker");
    }

    private void OnEnable()
    {
        colorPickerControl = FindObjectOfType<ColorPickerControl>();
        if (colorPickerControl == null)
        {
            Debug.LogWarning("ColorPickerControl not found in the scene. Make sure the ColorPickerControl script is attached to a GameObject in the scene.");
        }
    }

    private void OnGUI()
    {
        if (colorPickerControl == null)
        {
            EditorGUILayout.LabelField("No ColorPickerControl found in the scene.");
            return;
        }

        EditorGUILayout.LabelField("Color Picker", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        float newHue = EditorGUILayout.Slider("Hue", colorPickerControl.currentHue, 0f, 1f);
        float newSat = EditorGUILayout.Slider("Saturation", colorPickerControl.currentSat, 0f, 1f);
        float newVal = EditorGUILayout.Slider("Value", colorPickerControl.currentVal, 0f, 1f);

        if (EditorGUI.EndChangeCheck())
        {
            colorPickerControl.currentHue = newHue;
            colorPickerControl.currentSat = newSat;
            colorPickerControl.currentVal = newVal;
            colorPickerControl.UpdateOutputTmage();
        }

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Save Color"))
        {
            colorPickerControl.SaveColor();
        }

        GUILayout.EndHorizontal();
    }
}
