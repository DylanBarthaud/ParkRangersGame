using UnityEngine;

public interface IInteractable
{
    public void OnInteract(Interactor interactor) { }
    public void OnInteractHeld(Interactor interactor, int tick) { }
    public void OnInteractReleased(Interactor interactor, int tick) { }
    public bool CanInteract(Interactor interactor) { return true; }
}
