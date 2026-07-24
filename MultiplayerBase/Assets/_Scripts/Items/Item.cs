using BlackboardSystem;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

    [Header("Battery Settings")]
    [SerializeField] protected BatteryList batteryList;
    [SerializeField] private string[] StartingBatteries; 
    [HideInInspector] public bool UsesBatteries = false;
    [HideInInspector] public int MaxBatteries;
    [HideInInspector] public BatteryType BatteryTypesTaken;
    private List<BatteryStruct> batteries = new();
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
    public List<BatteryStruct> Batteries => batteries;
    public int CurrentPower
    {
        get
        {
            if(batteries.Count <= 0) return 0;
            return batteries[index].Power;
        }
    }
    public int Index => index;

    private void Awake()
    {
        gFXHandler = GetComponent<GFXHandler>();

        if (UsesBatteries)
            EventManager.instance.onTick_5 += UsePower; 
    }

    public override void OnNetworkSpawn()
    {
        if(!IsHost) return;

        foreach (var batteryName in StartingBatteries)
        {
            Battery battery = ObjectPools.Instance.SpawnBattery(ObjectPools.Instance.BatteryPool.BatteryPrefab);
            AddBatteryClientRPC(batteryName, battery.batteryPower, battery.Id);
        }
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
            if (removeOnPickUp)
            {
                //GetComponent<Collider>().enabled = false;
                SetItemColliderServerRpc(false);
            }
        }
    }

    [SerializeField] int index = 0;
    public virtual void UsePower(int tick) 
    {
        if(batteries.Count <=  0) return;
        if (!usingPower || batteries[index].Power == 0) return;
        BatteryStruct battery = batteries[index];
        battery.Power--;
        batteries[index] = battery;
        if (batteries[index].Power == 0) RunOutOfPower();
    }

    public virtual bool RunOutOfPower() 
    {
        if (batteries.Count > index +1)
        {
            index++; 
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
        usingPower = false;

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
        Debug.Log("Set collieder false"); 
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

    [ServerRpc(RequireOwnership = false)]
    public void EnableCarriedItemGFXServerRPC(bool enable, BlackboardKey holderKey) => EnableCarriedItemGFXClientRPC(enable, holderKey);
    [ClientRpc]
    private void EnableCarriedItemGFXClientRPC(bool enable, BlackboardKey holderKey)
    {
        if(enable)
        {
            if (BlackboardController.instance.GetBlackboard().TryGetValue(holderKey, out PlayerInfo playerInfo)) itemHeldLoc = playerInfo.heldItemPos;
        }
        else itemHeldLoc = null;

        isBeingHeld = enable;
        StartCoroutine(WaitForItemPosToUpdate(enable));
    }

    private System.Collections.IEnumerator WaitForItemPosToUpdate(bool enable)
    {
        yield return new WaitForEndOfFrame();
        if (enable) GFXHandler.EnableGFX("ItemGFX");
        else GFXHandler.DisableGFX("ItemGFX");
    }

    #region Battery Logic
    public bool AddBattery(string batteryName, int currentPower, int id)
    {
        BatteryStruct batteryStruct = batteryList.GetBattery(batteryName); 
        if ((BatteryTypesTaken & batteryStruct.BatteryType) == 0
            || batteries.Count >= MaxBatteries) return false;
        AddBatteryServerRPC(batteryStruct.Name, currentPower, id);
        return true;
    }
    public virtual void RemoveBattery(int index, Vector3 dropBatteryPos, Inventory inventory)
    {
        ObjectPools.Instance.BatteryPool.GetBattery(batteries[index].Id).DropItem(dropBatteryPos, inventory); 
        RemoveBatteryServerRPC(index);
    }

    [ServerRpc(RequireOwnership =false)]
    private void AddBatteryServerRPC(string batteryName, int currentPower, int id) => AddBatteryClientRPC(batteryName, currentPower, id);
    [ClientRpc]
    private void AddBatteryClientRPC(string batteryName, int currentPower, int id)
    {
        BatteryStruct newBattery = batteryList.GetBattery(batteryName);
        newBattery.startingPower = currentPower;
        newBattery.Id = id;
        newBattery.Power = newBattery.StartingPower;
        batteries.Add(newBattery);
        if (batteries.Count > 0 && CurrentPower == 0) index++; 
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveBatteryServerRPC(int index) => RemoveBatteryClientRPC(index);
    [ClientRpc]
    private void RemoveBatteryClientRPC(int index)
    {
        ObjectPools.Instance.BatteryPool.GetBattery(batteries[index].Id).batteryPower = batteries[index].Power;
        if (this.index >= batteries.Count -1 && batteries.Count > 1) this.index--;
        batteries.RemoveAt(index);
    }

    [ServerRpc(RequireOwnership = false)]
    protected void SetUsingPowerServerRPC(bool isUsing) => SetUsingPowerClientRPC(isUsing);
    [ClientRpc]
    private void SetUsingPowerClientRPC(bool isUsing) => usingPower = isUsing;
    #endregion
}

#if UNITY_EDITOR

[CustomEditor(typeof(Item), true)]
public class Item_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var item = (Item)target;

        EditorGUI.BeginChangeCheck();

        //EditorGUILayout.Space();
        //EditorGUILayout.LabelField("Battery Settings", EditorStyles.boldLabel);

        item.UsesBatteries = EditorGUILayout.Toggle("Uses Batteries", item.UsesBatteries);

        if (!item.UsesBatteries) return; 

        item.MaxBatteries = EditorGUILayout.IntField("Max Batteries", item.MaxBatteries);
        item.BatteryTypesTaken = (BatteryType)EditorGUILayout.EnumFlagsField("Battery Types Taken", item.BatteryTypesTaken);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(item, "Modify Item");
            EditorUtility.SetDirty(item);
        }
    }
}
#endif