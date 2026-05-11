using UnityEngine;

public class TorchItem : Item
{
    PlayerInfoHolder playerInfoHolder;

    public override void OnInteract(Interactor interactor, ItemType itemType = ItemType.None)
    {
        base.OnInteract(interactor, itemType);
        if (playerInfoHolder == null) playerInfoHolder = interactor.GetComponent<PlayerInfoHolder>();
        playerInfoHolder.ActivateTorchServerRPC(true);
    }

    public override void UseItem(GameObject user)
    {
        playerInfoHolder.ActivateTorchServerRPC();
    }

    public override void DropItem(Vector3 newPos, Inventory inventory )
    {
        base.DropItem(newPos, inventory);

        if (!inventory.hasItemOfType<TorchItem>())
        {
            playerInfoHolder.ActivateTorchServerRPC(false); 
        }
    }
}
