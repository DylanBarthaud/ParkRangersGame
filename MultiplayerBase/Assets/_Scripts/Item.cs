using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Item : NetworkBehaviour, IInteractable
{
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
        if (gFXHandler != null && removeOnPickUp) gFXHandler.EnableGFXClientRpc("ItemGFX");
        if (removeOnPickUp) GetComponent<Collider>().enabled = true;
        transform.position = newPos; 
    }

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
            if(!interactorInventory.AddItemToInventory(this)) return; 
            if (gFXHandler != null && removeOnPickUp) gFXHandler.DisableGFXClientRpc("ItemGFX"); 
            if (removeOnPickUp) GetComponent<Collider>().enabled = false;
        }
    }
}