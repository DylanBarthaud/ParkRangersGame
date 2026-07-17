using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
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

    [SerializeField] protected bool removeOnPickUp = true;
    [SerializeField] private int throwForceFoward, throwForceup;
    [Header("Item Audio")]
    [SerializeField] private bool hasAudio = false;
    [SerializeField] protected float audioVolume = 1; 
    [SerializeField] private string audioName;
    [Header("Item GFX")]
    [SerializeField] private GFXHandler gFXHandler;
    [SerializeField] private Vector3 heldItemLocOffset = Vector3.zero;

    [HideInInspector] public bool UsesBatteries = false;
    [HideInInspector] public int MaxBatteries;
    [HideInInspector] public BatteryType BatteryTypesTaken;
    [SerializeField] private BatterySO[] batteries;
    [SerializeField] private List<int> batteryIds = new(); 
    [SerializeField] protected int currentPower;
    protected bool usingPower; 

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
    public int CurrentPower => currentPower;

    private void Awake()
    {
        gFXHandler = GetComponent<GFXHandler>();

        if(UsesBatteries && batteryIds.Count > 0)
        {
            Debug.Log("HERE"); 
            currentPower = batteries[batteryIds[0]].Power;
        }

        if (UsesBatteries)
            EventManager.instance.onTick_5 += UsePower; 
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

    public virtual void UsePower(int tick) 
    {
        if (!usingPower || currentPower == 0) return;
        currentPower--;
        if (currentPower == 0) RunOutOfPower();
    }

    public virtual bool RunOutOfPower() 
    {
        batteryIds.RemoveAt(0);
        if (batteryIds.Count > 0)
        {
            currentPower = batteries[batteryIds[0]].Power; 
            return false;
        }
        return true;
    }

    public bool RequiresZoneCheckIn() { return false; }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(Vector3 newPos, Vector3 foward)
    {
        SetItemColliderClientRpc(true);
        DropItemClientRpc(newPos, foward);
        if (gFXHandler != null && removeOnPickUp) gFXHandler.EnableGFXServerRpc("ItemGFX");
    }

    [ClientRpc]
    private void DropItemClientRpc(Vector3 newPos, Vector3 foward)
    {
        transform.position = newPos;

        Rigidbody rb = GetComponent<Rigidbody>();

        GetComponent<Rigidbody>().useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        GetComponent<Rigidbody>().AddForce(Vector3.up * throwForceup, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(foward * throwForceFoward, ForceMode.Impulse);
    }

    [ServerRpc(RequireOwnership = false)] 
    protected void SetItemColliderServerRpc(bool enabled)
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

    public bool AddBattery(BatterySO batterySO)
    {
        if ((BatteryTypesTaken & batterySO.BatteryType) == 0
            || batteryIds.Count >= MaxBatteries) return false;
        AddBatteryServerRPC(batterySO.Id);
        return true;
    }
    public void RemoveBattery(int index) => RemoveBatteryServerRPC(index);

    [ServerRpc(RequireOwnership =false)]
    private void AddBatteryServerRPC(int batteryId) => AddBatteryClientRPC(batteryId);
    [ClientRpc]
    private void AddBatteryClientRPC(int batteryId)
    {
        batteryIds.Add(batteryId);
        if(batteryIds.Count == 1) 
            currentPower = batteries[batteryIds[0]].Power;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveBatteryServerRPC(int batteryId) => RemoveBatteryClientRPC(batteryId);
    [ClientRpc]
    private void RemoveBatteryClientRPC(int batteryId) => batteryIds.Add(batteryId);
}

[CustomEditor(typeof(Item), true)]
public class Item_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var item = (Item)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Battery Settings", EditorStyles.boldLabel);

        item.UsesBatteries = EditorGUILayout.Toggle("Uses Batteries", item.UsesBatteries);

        if (!item.UsesBatteries) return; 

        item.MaxBatteries = EditorGUILayout.IntField("Max Batteries", item.MaxBatteries);
        item.BatteryTypesTaken = (BatteryType)EditorGUILayout.EnumFlagsField("Battery Types Taken", item.BatteryTypesTaken);
    }
}