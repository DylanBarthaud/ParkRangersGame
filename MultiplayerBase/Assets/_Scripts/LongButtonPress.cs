using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pointerDown = false;
    private float pointerDownTimer = 0;

    [SerializeField] private float maxHoldTime;
    public UnityEvent onLongClick;

    [Header("UI")]
    [SerializeField] private Slider Slider;

    private void Awake() => Slider.maxValue = maxHoldTime;
    public void OnPointerDown(PointerEventData eventData) => pointerDown = true;

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
        Slider.value = 0;
        pointerDownTimer = 0; 
        Debug.Log("PointerUp");
    }

    private void Update()
    {
        if (pointerDown)
        {
            pointerDownTimer += Time.deltaTime; 
            if(pointerDownTimer >= maxHoldTime) onLongClick.Invoke();
            
            Slider.value = pointerDownTimer;
        }
    }
}