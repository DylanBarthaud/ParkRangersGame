using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private bool removeOnPickUp = true; 
    public Sprite Sprite => sprite;

    public abstract void UseItem();
    public void DropItem() { }

    public bool CanInteract(Interactor interactor)
    {
        if (interactor.gameObject.GetComponent<Inventory>() != null) return true;
        return false;
    }

    public void OnInteract(Interactor interactor)
    {
        Inventory interactorInventory = interactor.gameObject.GetComponent<Inventory>();
        if (interactorInventory != null)
        {
            interactorInventory.AddItemToInventory(this);
            GFXHandler gFXHandler = GetComponent<GFXHandler>();
            if (gFXHandler != null && removeOnPickUp) gFXHandler.DisableGFXClientRpc("ItemGFX"); 
            if (removeOnPickUp) GetComponent<Collider>().enabled = false;
        }
    }
}