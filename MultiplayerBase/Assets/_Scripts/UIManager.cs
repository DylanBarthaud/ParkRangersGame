using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public TextMeshProUGUI ButtonsPressedText;
    [SerializeField] private GameObject SpectatePanel;
    private void Awake()
    {
        slider.gameObject.SetActive(false);

        EventManager.instance.onButtonHeld += OnButtonHeld;
        EventManager.instance.onPuzzleComplete += SetSliderFalse;
        EventManager.instance.onButtonReleased += SetSliderFalseWrap; 
    }

    private void SetSliderFalseWrap()
    {
        SetSliderFalse(); 
    }

    private void OnButtonHeld(int tick, Interactor interactor)
    {
        slider.gameObject.SetActive(true);
        slider.value = tick;
    }

    public void SetSliderFalse(bool s = true, IInteractable puzzle = null)
    {
        slider.value = 0;
        slider.gameObject.SetActive(false);
    }

    public void SetSpectatePanelOn()
    {
        SpectatePanel.SetActive(true);
    }

    public void EnableMiniGameUi(GameObject MiniGamePanel, GameObject caller)
    {
        MiniGamePanel.SetActive(true);
        MiniGamePanel.GetComponent<MiniGameBase>().SetMiniGameObj(caller);
    }

    public void DisableMiniGameUi(GameObject MiniGamePanel)
    {
        MiniGamePanel?.SetActive(false);
    }
}
