using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public TextMeshProUGUI ButtonsPressedText; 

    private void Awake()
    {
        slider.gameObject.SetActive(false);

        EventManager.instance.onButtonHeld += OnButtonHeld;
        EventManager.instance.onButtonPressed += SetSliderFalse;
        EventManager.instance.onButtonReleased += SetSliderFalse; 
    }

    private void OnButtonHeld(int tick, Interactor interactor)
    {
        slider.gameObject.SetActive(true);
        slider.value = tick;
    }

    public void SetSliderFalse()
    {
        slider.value = 0;
        slider.gameObject.SetActive(false);
    }
}
