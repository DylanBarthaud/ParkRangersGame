using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{

    public Image BrightnessImage;
    public Slider BrightnessSlider;

    public Color BrightnessColor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeBrightness();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ChangeBrightness()
    {

        float i = 1 - BrightnessSlider.value;

        BrightnessImage.color = new Color(0, 0, 0, i);

    }
}
