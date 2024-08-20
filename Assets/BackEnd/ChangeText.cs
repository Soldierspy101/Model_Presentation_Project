using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeText : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI buttonText; // Reference to the TMP text component

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the onValueChanged event of the toggle
        toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(toggle);
        });

        // Set the initial text of the button based on the initial state of the toggle
        if (toggle.isOn)
        {
            buttonText.text = "Assemble";
        }
        else
        {
            buttonText.text = "Disassemble";
        }
    }

    // Function to handle toggle value change
    void ToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            buttonText.text = "Assemble";
        }
        else
        {
            buttonText.text = "Disassemble";
        }
    }
}
