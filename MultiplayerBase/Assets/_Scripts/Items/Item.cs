using System;
using Unity.Netcode;
using UnityEngine;

public enum ItemType { None, WaterBucket, ScrewDriver, Fuse, Fuel } 

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(GFXHandler))]
public abstract class Item : NetworkBehaviour, IInteractable
{
    [Header("Base Item Settings")]
    [SerializeField] private ItemType itemType;
    public ItemType ItemType => itemType;

    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;
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
    [SerializeField] private int throwForceFoward, throwForceup; 
    [SerializeField] private GFXHandler gFXHandler;

    private bool canPickUpItem = true; 

    private void Awake()
    {
        gFXHandler = GetComponent<GFXHandler>();
    }

    public abstract void UseItem(GameObject user);
    public virtual void DropItem(Vector3 newPos, Inventory inventory) 
    {
        if (gFXHandler != null && removeOnPickUp) DropItemServerRpc(newPos, Camera.main.transform.forward);
    }

    public virtual bool CanInteract(Interactor interactor, ItemType itemType = ItemType.None)
    {
        if (interactor.gameObject.GetComponent<Inventory>() != null && canPickUpItem) return true;
        return false;
    }

    public virtual void OnInteract(Interactor interactor, ItemType itemType = ItemType.None)
    {
        Inventory interactorInventory = interactor.gameObject.GetComponent<Inventory>();
        if (interactorInventory != null)
        {
            if (!interactorInventory.AddItemToInventory(this)) return; 
            if (gFXHandler != null && removeOnPickUp) gFXHandler.DisableGFXServerRpc("ItemGFX"); 
            if (removeOnPickUp)
            {
                GetComponent<Collider>().enabled = false;
                SetItemColliderServerRpc(false);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(Vector3 newPos, Vector3 foward)
    {
        SetItemColliderClientRpc(true);
        DropItemClientRpc();

        transform.position = newPos;

        Rigidbody rb = GetComponent<Rigidbody>();

        GetComponent<Rigidbody>().useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        GetComponent<Rigidbody>().AddForce(Vector3.up * throwForceup, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(foward * throwForceFoward, ForceMode.Impulse);
    }

    [ClientRpc]
    private void DropItemClientRpc()
    {
        if (gFXHandler != null && removeOnPickUp) gFXHandler.EnableGFXServerRpc("ItemGFX");
    }

    [ServerRpc(RequireOwnership = false)] 
    private void SetItemColliderServerRpc(bool enabled)
    {
        SetItemColliderClientRpc(enabled);
    }

    [ClientRpc]
    private void SetItemColliderClientRpc(bool enabled)
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Collider>().enabled = enabled;
    }

    [ServerRpc(RequireOwnership = false)]
    protected void SetCanPickUpItemServerRPC(bool canPickUp) => SetCanPickUpItemClientRPC(canPickUp);
    [ClientRpc]
    private void SetCanPickUpItemClientRPC(bool canPickUp)
    {
        canPickUpItem = canPickUp;
    }
}