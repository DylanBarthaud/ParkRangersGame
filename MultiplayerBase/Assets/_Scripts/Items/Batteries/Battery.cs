using UnityEngine;

public class Battery : Item, IInteractable
{
    [Header("Battery Settings")]
    [SerializeField] private BatterySO batterySO; 

    public override void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        Inventory interactorInventory = interactor.gameObject.GetComponent<Inventory>();
        if (interactorInventory.AddBattery(batterySO))
        {
            if (GFXHandler != null && removeOnPickUp) GFXHandler.DisableGFXServerRpc("ItemGFX");
            if (removeOnPickUp)
            {
                GetComponent<Collider>().enabled = false;
                SetItemColliderServerRpc(false);
            }
            return;
        }

        base.OnInteract(interactor, itemUsed);
    }

    public override void UseItem(GameObject user)
    {
    }
}
