using UnityEngine;
using UnityEngine.UI;

public class BatteryUi : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Slider slider;

    public void Initilize(BatteryStruct battery)
    {
        Debug.Log(battery.Power);

        image.sprite = battery.Icon;
        slider.maxValue = battery.MaxPower;
        slider.value = battery.Power;
    }

    public void UpdateSlider(int newVal) => slider.value = newVal;
}
