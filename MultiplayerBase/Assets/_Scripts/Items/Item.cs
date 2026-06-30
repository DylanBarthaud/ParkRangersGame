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
    [SerializeField] private Sprite sprite;
    [SerializeField] private bool infiniteUses = true; 
    [SerializeField] protected int uses;
    [SerializeField] private bool isHeavy = false;
    [SerializeField] private bool removeOnPickUp = true;
    [SerializeField] private int throwForceFoward, throwForceup;
    [Header("Item Audio")]
    [SerializeField] private bool hasAudio = false;
    [SerializeField] protected float audioVolume = 1; 
    [SerializeField] private string audioName;
    [Header("Item GFX")]
    [SerializeField] private GFXHandler gFXHandler;
    [SerializeField] private Vector3 heldItemLocOffset = Vector3.zero; 

    [HideInInspector] public bool isBeingHeld;
    [HideInInspector] public Transform itemHeldLoc;
    private bool canPickUpItem = true;

    public ItemType ItemType => itemType;
    public Sprite Sprite => sprite;
    public bool InfiniteUses => infiniteUses;
    public int Uses => uses;
    public bool IsHeavy => isHeavy;
    public bool HasAudio => hasAudio;
    public float AudioVolume => audioVolume;
    public string AudioName => audioName;
    public GFXHandler GFXHandler => gFXHandler;

    private void Awake()
    {
        gFXHandler = GetComponent<GFXHandler>();
    }

    protected void Update()
    {
        if (isBeingHeld)
        {
            transform.position = itemHeldLoc.TransformPoint(heldItemLocOffset);
            transform.rotation = itemHeldLoc.rotation;
        }
    }

    public abstract void UseItem(GameObject user);
    public virtual void DropItem(Vector3 newPos, Inventory inventory) 
    {
        if (gFXHandler != null && removeOnPickUp) DropItemServerRpc(newPos, Camera.main.transform.forward);
    }

    public virtual (bool, string) CanInteract(Interactor interactor, ItemType itemType = ItemType.None)
    {
        if (interactor.gameObject.GetComponent<Inventory>() != null && canPickUpItem) return (true, "");
        return (false, "No inventory space");
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
    public bool RequiresZoneCheckIn() { return false; }

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