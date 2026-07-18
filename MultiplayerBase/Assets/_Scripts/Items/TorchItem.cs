using UnityEngine;

public class TorchItem : Item
{
    PlayerInfoHolder playerInfoHolder;

    public override void OnInteract(Interactor interactor, ItemType itemType = ItemType.None)
    {
        base.OnInteract(interactor, itemType);
        if (playerInfoHolder == null) playerInfoHolder = interactor.GetComponent<PlayerInfoHolder>();
    }

    public override void UseItem(GameObject user)
    {
        if(CurrentPower <= 0) return;

        playerInfoHolder.ActivateTorchServerRPC(!playerInfoHolder.isTorchActive);
        usingPower = playerInfoHolder.isTorchActive;
    }

    public override void DropItem(Vector3 newPos, Inventory inventory )
    {
        base.DropItem(newPos, inventory);
        playerInfoHolder.ActivateTorchServerRPC(false);
    }

    public override bool RunOutOfPower()
    {
        if(!base.RunOutOfPower()) return false;
        playerInfoHolder.ActivateTorchServerRPC(false);
        return true;
    }
}
