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
    private void OnEnable() => Slider.value = 0;

    public void OnPointerDown(PointerEventData eventData) => pointerDown = true;

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
        Slider.value = 0;
        pointerDownTimer = 0; 
    }

    private void Update()
    {
        if (pointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            Slider.value = pointerDownTimer;

            if (pointerDownTimer >= maxHoldTime)
            {
                onLongClick.Invoke();
                pointerDownTimer = 0;
                pointerDown = false;
            }
        }
    }
}