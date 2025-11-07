using UnityEngine;

public interface IInteractable
{
    public void OnInteract(Interactor interactor);
    public bool CanInteract(Interactor interactor); 
}
