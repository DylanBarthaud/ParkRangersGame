using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pointerDown = false;
    private float pointerDownTimer = 0;

    [SerializeField] private float requiredHoldTime;
    public UnityEvent onLongClick; 


    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        Debug.Log("PointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
        Debug.Log("PointerUp");
    }

    private void Update()
    {
        if (pointerDown)
        {
            pointerDownTimer += Time.deltaTime; 
            if(pointerDownTimer >= requiredHoldTime) onLongClick.Invoke();
        }
    }
}
