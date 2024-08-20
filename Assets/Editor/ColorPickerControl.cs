using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ColorPickerControl : MonoBehaviour
{
    // Start is called before the first frame update
    public float currentHue, currentSat, currentVal;
    [SerializeField]
    public RawImage hueImage, satValImage, outputImage;

    [SerializeField]
    public Slider hueSlider;

    [SerializeField]
    public TMP_InputField hexInputField;

    public Texture2D hueTexture, svTexture, outputTexture;

    [SerializeField]
    Renderer changeThisColour;

    [SerializeField]
    Material material;

    public void Start()
    {
        LoadSavedColor(); // Load the saved color when the script starts

        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();
        UpdateOutputTmage();
    }

    public void LoadSavedColor()
    {
        // Load the saved color from PlayerPrefs
        float savedHue = PlayerPrefs.GetFloat("SavedHue", 0f);
        float savedSat = PlayerPrefs.GetFloat("SavedSat", 0f);
        float savedVal = PlayerPrefs.GetFloat("SavedVal", 0f);

        // Set the current color values to the saved values
        currentHue = savedHue;
        currentSat = savedSat;
        currentVal = savedVal;
    }

    public void SaveColor()
    {
        // Save the current color values to PlayerPrefs
        PlayerPrefs.SetFloat("SavedHue", currentHue);
        PlayerPrefs.SetFloat("SavedSat", currentSat);
        PlayerPrefs.SetFloat("SavedVal", currentVal);
        PlayerPrefs.Save(); // Save the PlayerPrefs data
    }

    public void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16); // Texture width is 1, height is 16
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        for (int i = 0; i < hueTexture.height; i++)
        {
            float hue = (float)i / (hueTexture.height - 1); // Use hueTexture.height - 1 to ensure the full hue range from 0 to 1
            Color color = Color.HSVToRGB(hue, 1, 1); // Set saturation and value to 1 for full color
            hueTexture.SetPixel(0, i, color);
        }

        hueTexture.Apply();
        currentHue = 0;
        hueImage.texture = hueTexture;
    }


    public void CreateSVImage()
    {
        svTexture = new Texture2D(16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        for(int y = 0; y < svTexture.height; y++)
        {
            for(int x = 0; x <svTexture.width; x++) 
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(
                                     currentHue,
                                     (float)x/ svTexture.width,
                                     (float)y/ svTexture.height));
            }
        }

        svTexture.Apply();
        currentSat = 0;
        currentVal = 0;

        satValImage.texture = svTexture;
    }

    public void CreateOutputImage()
    {
        outputTexture = new Texture2D(1,16);
        outputTexture.wrapMode = TextureWrapMode.Clamp;
        outputTexture.name = "OutputTexture";

        Color currentColour = Color.HSVToRGB(currentHue, currentSat, currentVal);
        for(int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColour);
        }
        outputTexture.Apply();
        outputImage.texture = outputTexture;
    }

    public void UpdateOutputTmage()
    {
        Color currentColour = Color.HSVToRGB(currentHue, currentSat, currentVal);
       
        for(int i = 0; i<outputTexture.height; i++)
        {
            outputTexture.SetPixel(0,i, currentColour);
        }
        outputTexture.Apply();

        hexInputField.text = ColorUtility.ToHtmlStringRGB(currentColour);
        material.color = currentColour;
        changeThisColour.material.color = currentColour;
        Debug.Log("Color Changed to: " + currentColour+", "+ changeThisColour.material.color);
        SaveColor();

    }

    public void SetSV(float S, float V)
    {
        currentSat = S;
        currentVal = V;
        SaveColor(); // Save the color when the user sets the saturation and value
        UpdateOutputTmage();
    }

    public void UpdateSVImage()
    {
        currentHue = hueSlider.value;

        for(int y =0; y <svTexture.height; y++)
        {
            for(int x =0; x <svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(
                                    currentHue,
                                    (float)x / svTexture.width,
                                    (float)y / svTexture.height));
            }
        }

        svTexture.Apply();

        UpdateOutputTmage();
    }

    public void OnTextInput()
    {
        if(hexInputField.text.Length < 6)
        {
            return;
        }
        Color newCol;

        if(ColorUtility.TryParseHtmlString("#"+hexInputField.text, out newCol))
            Color.RGBToHSV(newCol,out currentHue, out currentSat, out currentVal);

        hueSlider.value = currentHue;

        hexInputField.text = "";

        UpdateOutputTmage() ;
    }

}
