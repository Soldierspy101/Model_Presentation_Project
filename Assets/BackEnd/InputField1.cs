using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldManager1 : MonoBehaviour
{
    public Text obj_text1; // First display text element
    public Text obj_text2; // Second display text element
    public InputField inputField; // Unity's input field
    private Text activeText; // Pointer to the currently active text element

    // Start is called before the first frame update
    void Start()
    {
        // Initialize activeText to obj_text1 by default
        activeText = obj_text1;
        inputField.text = activeText.text; // Set the input field to the value of the active text
    }

    public void SwitchActiveText()
    {
        // Toggle the active text component between obj_text1 and obj_text2
        activeText = (activeText == obj_text1) ? obj_text2 : obj_text1;

        // Update the input field to reflect the text of the newly active text box
        inputField.text = activeText.text;
    }

    public void UpdateText()
    {
        // Update the active text component from the input field
        activeText.text = inputField.text;

        // Optionally, save each text separately if persistence is required
        PlayerPrefs.SetString("Description1", obj_text1.text);
        PlayerPrefs.SetString("Description2", obj_text2.text);
        PlayerPrefs.Save();
    }
}
