using UnityEngine;

public class Battery : Item, IInteractable
{
    private int id; 
    [Header("Battery Settings")]
    [SerializeField] private string batteryName;
    [HideInInspector] public int batteryPower; 
    public string Name => batteryName;
    public int Id => id;
    public override void OnNetworkSpawn()
    {
        id = GetInstanceID();
        ObjectPools.Instance.BatteryPool.AddObjToPool(this);
        batteryPower = batteryList.GetBattery(batteryName).StartingPower;
    }

    public override void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        Inventory interactorInventory = interactor.gameObject.GetComponent<Inventory>();
        if (interactorInventory.AddBattery(batteryName, batteryPower, id))
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
