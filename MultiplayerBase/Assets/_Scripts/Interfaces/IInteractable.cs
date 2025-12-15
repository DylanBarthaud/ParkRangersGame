using UnityEngine;

public interface IInteractable
{
    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None);
    public void OnInteractHeld(Interactor interactor, int tick, ItemType itemUsed = ItemType.None) { }
    public void OnInteractReleased(Interactor interactor, int tick, ItemType itemUsed = ItemType.None) { }
    public bool CanInteract(Interactor interactor, ItemType itemUsed = ItemType.None) { return true; }
    public IInteractable GetInteractable() { return this; }
}
