using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private VisualElement _optionsContainer;
    private Button _helpButton;
    private Button _infoButton;
    private Button _textButton;
    private Button _disassembleButton;
    private Button _designButton;
    private Button _menuButton;
    private Button _closeButton;
    private VisualElement _tooltipsContainer;
    private Label _helpTooltip;
    private Label _infoTooltip;
    private Label _textTooltip;
    private Label _disassembleTooltip;
    private Label _designTooltip;

    private GameObject help;
    private GameObject info;
    private GameObject tv;
    private GameObject text;

    private GameObject annotation;
    //private AnimationScript1 disassemble;
    // Find the GameObject called "AddModelHere"
    private GameObject parentObject;
    private Animator animation;
    private GameObject design;

    // Start is called before the first frame update
    void Start()
    {
        //Grab the topmost visual element in the UI Document
        var root = GetComponent<UIDocument>().rootVisualElement;

        //Assign the variables
        _optionsContainer = root.Q<VisualElement>("options_container");
        _helpButton = root.Q<Button>("Help");
        _infoButton = root.Q<Button>("Info");
        _textButton = root.Q<Button>("Text");
        _disassembleButton = root.Q<Button>("Disassemble");
        _designButton = root.Q<Button>("Design");
        _menuButton = root.Q<Button>("menu");
        _closeButton = root.Q<Button>("close");
        _tooltipsContainer = root.Q<VisualElement>("tooltips_container");
        _helpTooltip = root.Q<Label>("Help");
        _infoTooltip = root.Q<Label>("Info");
        _textTooltip = root.Q<Label>("Text");
        _disassembleTooltip = root.Q<Label>("Disassemble");
        _designTooltip = root.Q<Label>("Design");

        help = GameObject.Find("UI/Functions/FirstFunction");
        info = GameObject.Find("UI/Functions/SecondFunction/Popup");
        tv = GameObject.Find("AddModelHere/Media/TV");
        text = GameObject.Find("UI/Functions/SecondFunction/Popup/Panel");
 
        annotation = GameObject.Find("/ModelCanvas");
        //disassemble = FindObjectOfType<AnimationScript1>();
        parentObject = GameObject.Find("/AddModelHere");
        // Find the child GameObject that has the Animator component
        animation = parentObject.GetComponentInChildren<Animator>();
        design = GameObject.Find("UI/Functions/FourthFunction");

        if (help == null)
        {
            Debug.LogError("Help GameObject not found.");
            return;
        }
        if (info == null)
        {
            Debug.LogError("Info GameObject not found.");
            return;
        }
        if (tv == null)
        {
            Debug.LogError("TV GameObject not found.");
            return;
        }
        if (text == null)
        {
            Debug.LogError("Text GameObject not found.");
            return;
        }
        if (annotation == null)
        {
            Debug.LogError("Annotation GameObject not found.");
            return;
        }
        if (parentObject == null)
        {
            Debug.LogError("Disassemble GameObject not found.");
            return;
        }
        if (design == null)
        {
            Debug.LogError("Design GameObject not found.");
            return;
        }

        //Hide the options and close button when the scene starts
        _optionsContainer.style.display = DisplayStyle.None;
        _closeButton.style.display = DisplayStyle.None;

        //Hide tooltips
        _tooltipsContainer.style.display = DisplayStyle.None;

        //Registering buttons' callback methods
        _menuButton.RegisterCallback<ClickEvent>(OnMenuButtonClicked);
        _closeButton.RegisterCallback<ClickEvent>(OnCloseButtonClicked);
        _helpButton.RegisterCallback<ClickEvent>(OnHelpButtonClicked);
        _infoButton.RegisterCallback<ClickEvent>(OnInfoButtonClicked);
        _textButton.RegisterCallback<ClickEvent>(OnTextButtonClicked);
        _disassembleButton.RegisterCallback<ClickEvent>(OnDisassembleButtonClicked);
        _designButton.RegisterCallback<ClickEvent>(OnDesignButtonClicked);

        _helpButton.RegisterCallback<MouseEnterEvent>(evt => ShowTooltip(_helpTooltip, _helpButton));
        _helpButton.RegisterCallback<MouseLeaveEvent>(evt => HideTooltip(_helpTooltip));
        _infoButton.RegisterCallback<MouseEnterEvent>(evt => ShowTooltip(_infoTooltip, _infoButton));
        _infoButton.RegisterCallback<MouseLeaveEvent>(evt => HideTooltip(_infoTooltip));
        _textButton.RegisterCallback<MouseEnterEvent>(evt => ShowTooltip(_textTooltip, _textButton));
        _textButton.RegisterCallback<MouseLeaveEvent>(evt => HideTooltip(_textTooltip));
        _disassembleButton.RegisterCallback<MouseEnterEvent>(evt => ShowTooltip(_disassembleTooltip, _disassembleButton));
        _disassembleButton.RegisterCallback<MouseLeaveEvent>(evt => HideTooltip(_disassembleTooltip));
        _designButton.RegisterCallback<MouseEnterEvent>(evt => ShowTooltip(_designTooltip, _designButton));
        _designButton.RegisterCallback<MouseLeaveEvent>(evt => HideTooltip(_designTooltip));
    }

    private void ShowTooltip(Label tooltipToShow, VisualElement button)
    {
        _tooltipsContainer.style.display = DisplayStyle.Flex;
        _helpTooltip.style.display = DisplayStyle.None;
        _infoTooltip.style.display = DisplayStyle.None;
        _textTooltip.style.display = DisplayStyle.None;
        _disassembleTooltip.style.display = DisplayStyle.None;
        _designTooltip.style.display = DisplayStyle.None;

        // Schedule the tooltip to be shown in the next frame
        StartCoroutine(ShowTooltipCoroutine(tooltipToShow, button));
    }

    private IEnumerator ShowTooltipCoroutine(Label tooltipToShow, VisualElement button)
    {
        // Wait for the next layout update
        yield return new WaitForEndOfFrame();

        // Calculate the tooltip position based on the button position
        var buttonWorldBound = button.worldBound;
        var rootWorldBound = _tooltipsContainer.worldBound;

        // Set tooltip position relative to its container
        tooltipToShow.style.left = buttonWorldBound.x - rootWorldBound.x + buttonWorldBound.width / 2 - tooltipToShow.resolvedStyle.width / 2 - 230;
        tooltipToShow.style.top = buttonWorldBound.y - rootWorldBound.y - tooltipToShow.resolvedStyle.height - 170;

        // Display the tooltip after positioning
        tooltipToShow.style.display = DisplayStyle.Flex;
    }

    private void HideTooltip(Label tooltipToHide)
    {
        tooltipToHide.style.display = DisplayStyle.None;
    }

    private void OnMenuButtonClicked(ClickEvent evt)
    {
        //Show the options and close button
        _optionsContainer.style.display = DisplayStyle.Flex;
        _closeButton.style.display = DisplayStyle.Flex;

        //Trigger the transition animations for the menu
        _optionsContainer.AddToClassList("menu-up");

    }

    private void OnCloseButtonClicked(ClickEvent evt)
    {
        //Hide the options and close button 
        _optionsContainer.style.display = DisplayStyle.None;
        _closeButton.style.display = DisplayStyle.None;

        //Trigger the transition animations for the menu
        _optionsContainer.RemoveFromClassList("menu-up");

    }

    private void ResetAllButtons()
    {
        _helpButton.RemoveFromClassList("help-active");
        _infoButton.RemoveFromClassList("info-active");
        _textButton.RemoveFromClassList("text-active");
        _disassembleButton.RemoveFromClassList("disassemble-active");
        _designButton.RemoveFromClassList("design-active");

        _helpButton.RemoveFromClassList("help-disable");
        _infoButton.RemoveFromClassList("info-disable");
        _textButton.RemoveFromClassList("text-disable");
        _disassembleButton.RemoveFromClassList("disassemble-disable");
        _designButton.RemoveFromClassList("design-disable");
    }

    private void SetActiveButton(Button activeButton, string activeClass)
    {
        ResetAllButtons();
        activeButton.AddToClassList(activeClass);

        if (_helpButton != activeButton) _helpButton.AddToClassList("help-disable");
        if (_infoButton != activeButton) _infoButton.AddToClassList("info-disable");
        if (_textButton != activeButton) _textButton.AddToClassList("text-disable");
        if (_disassembleButton != activeButton) _disassembleButton.AddToClassList("disassemble-disable");
        if (_designButton != activeButton) _designButton.AddToClassList("design-disable");
    }

    private void OnHelpButtonClicked(ClickEvent evt)
    {
        if (!_helpButton.ClassListContains("help-active"))
        {
            SetActiveButton(_helpButton, "help-active");
        }
        else
        {
            ResetAllButtons();
        }
    }

    private void OnInfoButtonClicked(ClickEvent evt)
    {

        if (!_infoButton.ClassListContains("info-active"))
        {
            SetActiveButton(_infoButton, "info-active");
            text.SetActive(true);

        }
        else
        {
            ResetAllButtons();
        }
    }


    private void OnTextButtonClicked(ClickEvent evt)
    {
        if (!_textButton.ClassListContains("text-active"))
        {
            SetActiveButton(_textButton, "text-active");
        }
        else
        {
            ResetAllButtons();
        }
    }

    private void OnDisassembleButtonClicked(ClickEvent evt)
    {
        if (!_disassembleButton.ClassListContains("disassemble-active"))
        {
            SetActiveButton(_disassembleButton, "disassemble-active");
        }
        else
        {
            ResetAllButtons();
        }
    }

    private void OnDesignButtonClicked(ClickEvent evt)
    {
        if (!_designButton.ClassListContains("design-active"))
        {
            SetActiveButton(_designButton, "design-active");
        }
        else
        {
            ResetAllButtons();
        }
    }

    private void Update()
    {
        // Help Button Logic
        if (_helpButton.ClassListContains("help-active"))
        {
            if (help != null)
            {
                help.SetActive(true);
            }
            else
            {
                Debug.LogError("Help GameObject is null.");
            }
        }
        else
        {
            if (help != null)
            {
                help.SetActive(false);
            }
        }

        // Info Button Logic
        if (_infoButton.ClassListContains("info-active"))
        {
            if (info != null)
            {
                info.SetActive(true);
            }
            else
            {
                Debug.LogError("Info GameObject is null.");
            }
            if (tv != null)
            {
                tv.SetActive(true);
            }
            else
            {
                Debug.LogError("TV GameObject is null.");
            }
        }
        else
        {
            if (info != null)
            {
                info.SetActive(false);
            }
            if (tv != null)
            {
                tv.SetActive(false);
            }
        }

        // Text Button Logic (assuming 'annotation' is intended for text-related logic)
        if (_textButton.ClassListContains("text-active"))
        {
            if (annotation != null)
            {
                annotation.SetActive(true);
            }
            else
            {
                Debug.LogError("Annotation GameObject is null.");
            }
        }
        else
        {
            if (annotation != null)
            {
                annotation.SetActive(false);
            }
        }

        // Disassemble Button Logic
        if (_disassembleButton.ClassListContains("disassemble-active"))
        {
            if (parentObject != null)
            {              
                animation.enabled = true;
                //animation.SetTrigger("Disassemble");
                //animation.ResetTrigger("Assemble");       
                animation.SetBool("Other", false);
                animation.SetFloat("Speed", 5);

            }
            else
            {
                Debug.LogError("AnimationScript1 component (Disassemble) is null.");
            }
        }
        else
        {
            if (parentObject != null)
            {
                //animation.SetTrigger("Assemble");
                //animation.ResetTrigger("Disassemble");
                animation.SetFloat("Speed", -5);
                animation.SetBool("Other", true);
            }
        }

        // Design Button Logic
        if (_designButton.ClassListContains("design-active"))
        {
            if (design != null)
            {
                design.SetActive(true);
            }
            else
            {
                Debug.LogError("Design GameObject is null.");
            }
        }
        else
        {
            if (design != null)
            {
                design.SetActive(false);
            }
        }
    }

}