using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Slider

public class CameraFOVAdjuster1 : MonoBehaviour
{
    public Camera specificCamera;
    public Slider fovSlider;
    public Text fovValueText;

    void Start()
    {
        if (specificCamera == null)
        {
            specificCamera = Camera.main;
        }

        if (fovSlider == null)
        {
            GameObject sliderObject = GameObject.Find("CAMERAZOOM/Slider");
            if (sliderObject != null)
            {
                fovSlider = sliderObject.GetComponent<Slider>();
            }
        }

        if (fovValueText == null)
        {
            GameObject textObject = GameObject.Find("CAMERAZOOM/Text");
            if (textObject != null)
            {
                fovValueText = textObject.GetComponent<Text>();
            }
        }

        if (fovSlider != null && specificCamera != null)
        {
            fovSlider.minValue = 15f;
            fovSlider.maxValue = 50f;
            fovSlider.value = specificCamera.fieldOfView;
            fovSlider.onValueChanged.AddListener(OnFOVSliderChanged);

            Debug.Log("FOV Slider initialized with value: " + specificCamera.fieldOfView);
        }
        else
        {
            return;
        }
    }


    public void OnFOVSliderChanged(float value)
    {
        if (specificCamera != null)
        {
            specificCamera.fieldOfView = value;

            if (fovValueText != null)
            {
                fovValueText.text = "FOV: " + value.ToString("F1");
            }

            Debug.Log("Camera FOV changed to: " + value);
        }
        else
        {
            Debug.LogError("Specific camera not assigned in FOV Slider change!");
        }
    }
}