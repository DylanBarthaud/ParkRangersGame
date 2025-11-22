using UnityEngine;

public class Test : MonoBehaviour, IInteractable
{
    public void OnInteract(Interactor interactor)
    {
        throw new System.NotImplementedException();
    }

    public void OnInteractHeld(Interactor interactor, int tick)
    {
        
    }

    public void OnInteractReleased(Interactor interactor, int tick) 
    { 
    
    }

    public bool CanInteract(Interactor interactor) 
    {
        throw new System.NotImplementedException();
    }
}