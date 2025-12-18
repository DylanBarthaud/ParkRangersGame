using Unity.Netcode;
using UnityEngine;

public enum ItemType { None, WaterBucket, ScrewDriver } 

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(GFXHandler))]
public abstract class Item : NetworkBehaviour, IInteractable
{
    [SerializeField] private ItemType itemType;
    public ItemType ItemType => itemType;

    [SerializeField] private Sprite sprite;
    [SerializeField] private bool infiniteUses = true; 
    public bool InfiniteUses => infiniteUses;
    [SerializeField] protected int uses;
    public int Uses => uses;

    [SerializeField] private bool hasAudio = false;
    public bool HasAudio => hasAudio;

    [SerializeField] protected float audioVolume = 1; 
    public float AudioVolume => audioVolume;

    [SerializeField] private string audioName;
    public string AudioName => audioName;

    [SerializeField] private bool removeOnPickUp = true; 
    public Sprite Sprite => sprite;

    [SerializeField] private GFXHandler gFXHandler;

    private void Awake()
    {
        gFXHandler = GetComponent<GFXHandler>();
    }

    public abstract void UseItem(GameObject user);
    public void DropItem(Vector3 newPos) 
    {
        if (gFXHandler != null && removeOnPickUp) gFXHandler.EnableGFXServerRpc("ItemGFX");
        if (removeOnPickUp) SetItemColliderServerRpc(true);
        transform.position = newPos; 
    }

    public bool CanInteract(Interactor interactor, ItemType itemType = ItemType.None)
    {
        if (interactor.gameObject.GetComponent<Inventory>() != null) return true;
        return false;
    }

    public void OnInteract(Interactor interactor, ItemType itemType = ItemType.None)
    {
        Inventory interactorInventory = interactor.gameObject.GetComponent<Inventory>();
        if (interactorInventory != null)
        {
            if (!interactorInventory.AddItemToInventory(this)) return; 
            if (gFXHandler != null && removeOnPickUp) gFXHandler.DisableGFXServerRpc("ItemGFX"); 
            if (removeOnPickUp) SetItemColliderServerRpc(false);
        }
    }

    [ServerRpc(RequireOwnership = false)] 
    private void SetItemColliderServerRpc(bool enabled)
    {
        SetItemColliderClientRpc(enabled);
    }

    [ClientRpc]
    private void SetItemColliderClientRpc(bool enabled)
    {
        GetComponent<Collider>().enabled = enabled;
    }
}