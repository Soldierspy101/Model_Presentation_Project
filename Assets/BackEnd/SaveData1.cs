using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveData1 : MonoBehaviour
{
    public Toggle[] toggles = new Toggle[5];
    public Text titleText;
    public Text descriptionText;

    void Start()
    {
        LoadAllToggleStates();
        LoadTextData();
        AutoFillTextFields();
    }

    void AutoFillTextFields()
    {
        GameObject uiObject = GameObject.Find("UI");
        if (uiObject != null)
        {
            GameObject secondFunctionObject = uiObject.transform.Find("SecondFunction")?.gameObject;
            if (secondFunctionObject != null)
            {
                GameObject popupObject = secondFunctionObject.transform.Find("Popup")?.gameObject;
                if (popupObject != null)
                {
                    Transform titlesTransform = popupObject.transform.Find("Titles");
                    Transform descriptionTransform = popupObject.transform.Find("Description");
                    if (titlesTransform != null && descriptionTransform != null)
                    {
                        titleText.text = titlesTransform.GetComponent<Text>()?.text;
                        descriptionText.text = descriptionTransform.GetComponent<Text>()?.text;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }
    }


    void LoadAllToggleStates()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            int toggleState = PlayerPrefs.GetInt("toggleState" + i, 0);
            if (toggles[i] != null)
            {
                toggles[i].isOn = toggleState == 1;
                toggles[i].onValueChanged.AddListener(delegate { SaveToggleState(i); });
            }
            else
            {
                Debug.LogError("Toggle at index " + i + " is not assigned!");
            }
        }
    }

    void LoadTextData()
    {
        // Load title and description from PlayerPrefs
        string title = PlayerPrefs.GetString("TextEditorWindow_Title", "Default Title");
        string description = PlayerPrefs.GetString("TextEditorWindow_Description", "Default Description");

        // Set the text of title and description objects
        titleText.text = title;
        descriptionText.text = description;
    }


    public void SaveToggleState(int index)
    {
        if (index >= 0 && index < toggles.Length && toggles[index] != null)
        {
            int toggleState = toggles[index].isOn ? 1 : 0;
            PlayerPrefs.SetInt("toggleState" + index, toggleState);
            PlayerPrefs.Save();
            Debug.Log("Saved state for Toggle " + index + ": " + toggles[index].isOn);
        }
    }
    
}