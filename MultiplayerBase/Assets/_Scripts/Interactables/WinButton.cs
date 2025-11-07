using UnityEngine;
using UnityEngine.EventSystems;

public class WinButton : MonoBehaviour, IInteractable
{
    public bool CanInteract(Interactor interactor)
    {
        return true; 
    }

    public void OnInteract(Interactor interactor)
    {
        EventManager.instance.OnButtonPressed(); 
        Destroy(gameObject);
    }
}
