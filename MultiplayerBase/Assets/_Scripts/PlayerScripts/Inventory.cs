using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject[] inventorySlots;
    [SerializeField] Sprite baseInvSlotSprite; 
    [SerializeField] private List<Item> items = new List<Item>();
    public List<Item> Items => items;
    [SerializeField] private int selectedItemSlot;
    public int SelectedItemSlot => selectedItemSlot;

    [HideInInspector] public bool canUseInv = true;

    [Header("UI Settings")]
    [SerializeField] private GameObject carryingHeavyGFX;
    [SerializeField] private Transform heldItemPos;
    [SerializeField] private Transform batteryContainer;
    [SerializeField] private GameObject batteryPrefab;
    private BatteryUi currentBatteryUi;
    private int batteryIndex = 0; 

    private bool carryingHeavy = false;
    public bool CarryingHeavy => carryingHeavy;
    private Item heavyItem = null;
    public Item HeavyItem => heavyItem; 

    private void Awake()
    {
        inventorySlots[0].GetComponent<Image>().color = Color.green;

        EventManager.instance.onPuzzleComplete += OnPuzzleComplete;
        EventManager.instance.onButtonReleased += EnableInv;
        EventManager.instance.onButtonHeld += OnButtonHeld;
        EventManager.instance.onTick_5 += OnTick;
    }
    private void OnTick(int obj)
    {
        if (items.Count <= 0 || !items[selectedItemSlot].UsesBatteries) return; 

        if(batteryIndex != items[selectedItemSlot].Index)
        {
            batteryIndex = items[selectedItemSlot].Index;
            for (int i = 0; i < batteryContainer.childCount; i++)
                Destroy(batteryContainer.GetChild(i).gameObject);
            EnableBatteryUi(items[selectedItemSlot], batteryIndex); 
        }

        if (items[selectedItemSlot].Batteries.Count > 0)
        {
            currentBatteryUi.UpdateSlider(items[selectedItemSlot].CurrentPower); 
        }

        else if((items[selectedItemSlot].UsesBatteries && currentBatteryUi != null))
        {
            currentBatteryUi.UpdateSlider(0); 
        }
    }

    private void OnButtonHeld(int arg1, Interactor interactor) => DisableInv();
    private void OnPuzzleComplete(bool arg1, IInteractable interactable) => EnableInv();

    public void EnableInv()
    {
        canUseInv = true;
    }

    public void DisableInv()
    {
        canUseInv = false;
    }

    private void Update()
    {
        if (!canUseInv) return;

        if (Input.GetKeyDown(KeyCode.Mouse0)
            && (items.Count > 0 || carryingHeavy))
        {
            Item selectedItem; 
            if (carryingHeavy) { selectedItem = heavyItem; }
            else
            {
                if (items[selectedItemSlot] == null) return;
                selectedItem = items[selectedItemSlot];
            }

            selectedItem.UseItem(gameObject);
            if (selectedItem.HasAudio) GetComponent<MultiplayerAudioHandlerWrapper>().PlaySoundServerRpc(selectedItem.AudioName, default, selectedItem.AudioVolume);
            if (!selectedItem.InfiniteUses && selectedItem.Uses <= 0)
            {
                EnableCarriedItemGFXServerRPC(false);
                RemoveItem(selectedItem);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) 
            && (items.Count > 0 || carryingHeavy))
        {
            Item selectedItem;
            if (carryingHeavy) { selectedItem = heavyItem; }
            else
            {
                if (items[selectedItemSlot] == null) return;
                selectedItem = items[selectedItemSlot];
            }

            RemoveItem(selectedItem);
            selectedItem.DropItem(gameObject.transform.position, this);
        }

        if(Input.GetKeyDown(KeyCode.R)
            && (items.Count > 0 || carryingHeavy))
        {
            Item selectedItem;
            if (carryingHeavy) { selectedItem = heavyItem; }
            else
            {
                if (items[selectedItemSlot] == null) return;
                selectedItem = items[selectedItemSlot];
            }

            if (selectedItem.UsesBatteries && selectedItem.Batteries.Count > 0)
            {
                selectedItem.RemoveBattery(0, gameObject.transform.position, this);
                for (int i = 0; i < batteryContainer.childCount; i++)
                    Destroy(batteryContainer.GetChild(i).gameObject);
                EnableBatteryUi(selectedItem, 0); 
            }
        }

        if(carryingHeavy) return;

        if (Input.mouseScrollDelta.y > 0 && selectedItemSlot < items.Count - 1)
        {
            DisableSlot(inventorySlots[selectedItemSlot], items[selectedItemSlot]);
            selectedItemSlot++;
            EnableSlot(inventorySlots[selectedItemSlot], items[selectedItemSlot]);
        }

        if (Input.mouseScrollDelta.y < 0 && selectedItemSlot > 0)
        {
            DisableSlot(inventorySlots[selectedItemSlot], items[selectedItemSlot]);
            selectedItemSlot--;
            EnableSlot(inventorySlots[selectedItemSlot], items[selectedItemSlot]);
        }
    }

    public bool CanAddItemToInv()
    {
        if (items.Count >= inventorySlots.Length) return false;
        return true; 
    }

    public bool AddItemToInventory(Item item)
    {
        if(items.Count >= inventorySlots.Length && !item.IsHeavy) return false;

        if (item.IsHeavy)
        {
            if(heavyItem !=  null) return false;

            if(items.Count > 0) EnableCarriedItemGFXServerRPC(false);

            carryingHeavy = true; 
            heavyItem = item;
            carryingHeavyGFX.SetActive(true);

            EnableCarriedItemGFXServerRPC(true);

            item.GFXHandler.ChangeGFXRenderLayerServerRpc("ItemGFX", 4);

            return true;
        }

        item.GFXHandler.ChangeGFXRenderLayerServerRpc("ItemGFX", 4);

        items.Add(item);
        if (items.Count == 1)
        {
            if (item.UsesBatteries) EnableBatteryUi(item, batteryIndex);
            EnableCarriedItemGFXServerRPC(true);
        }
        else item.GFXHandler.DisableGFXServerRpc("ItemGFX");

            inventorySlots[items.Count - 1].transform.GetChild(0).GetComponent<Image>().sprite = item.Sprite;
        return true;
    }

    public void RemoveItem(Item item)
    {
        item.isBeingHeld = false;
        item.GFXHandler.ChangeGFXRenderLayerServerRpc("ItemGFX", 2);

        for (int i = 0; i < batteryContainer.childCount; i++)
            Destroy(batteryContainer.GetChild(i).gameObject);

        if (item.IsHeavy)
        {
            carryingHeavy = false;
            heavyItem = null;
            carryingHeavyGFX.SetActive(false);

            if (items.Count <= 0) return; 
            
            EnableCarriedItemGFXServerRPC(true);
            if (items[selectedItemSlot].UsesBatteries) EnableBatteryUi(items[selectedItemSlot], batteryIndex);

            return;
        }

        inventorySlots[items.Count - 1].transform.GetChild(0).GetComponent<Image>().sprite = baseInvSlotSprite;
        if (selectedItemSlot > 0)
        {
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.gray;
            selectedItemSlot--;
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.green;
            if (items[selectedItemSlot].UsesBatteries) EnableBatteryUi(items[selectedItemSlot], batteryIndex);
        }

        items.Remove(item);

        int index = 0;
        foreach (var currentItem in items)
        {
            inventorySlots[index].transform.GetChild(0).GetComponent<Image>().sprite = currentItem.Sprite;
            index++;
        }

        if (items.Count > 0) EnableCarriedItemGFXServerRPC(true);
    }

    private void EnableSlot(GameObject slot, Item itemInSlot)
    {
        slot.GetComponent<Image>().color = Color.green;
        EnableCarriedItemGFXServerRPC(true);

        if (itemInSlot.UsesBatteries) EnableBatteryUi(itemInSlot, batteryIndex);
    }

    private void DisableSlot(GameObject slot, Item itemInSlot)
    {
        slot.GetComponent<Image>().color = Color.gray;
        EnableCarriedItemGFXServerRPC(false);

        for(int i = 0; i  < batteryContainer.childCount; i++) Destroy(batteryContainer.GetChild(i).gameObject);
    }

    private void EnableBatteryUi(Item item, int index)
    {
        int i = 0;
        foreach (var batteryStruct in item.Batteries)
        {
            BatteryUi newBatteryUI = Instantiate(batteryPrefab, batteryContainer).GetComponent<BatteryUi>();
            newBatteryUI.Initilize(batteryStruct);

            if (i == index) currentBatteryUi = newBatteryUI;
            i++;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void EnableCarriedItemGFXServerRPC(bool enable) => EnableCarriedItemGFXClientRPC(enable);
    [ClientRpc]
    private void EnableCarriedItemGFXClientRPC(bool enable)
    {
        Item item = carryingHeavy ? heavyItem : items[selectedItemSlot]; 
        item.isBeingHeld = enable;
        if (enable) item.itemHeldLoc = heldItemPos;
        StartCoroutine(WaitForItemPosToUpdate(item, enable));
    }

    private System.Collections.IEnumerator WaitForItemPosToUpdate(Item item, bool enable)
    {
        yield return new WaitForEndOfFrame();
        if(enable) item.GFXHandler.EnableGFX("ItemGFX");
        else item.GFXHandler.DisableGFX("ItemGFX");
    }

    public bool hasItem(Item item)
    {
        if(items.Contains(item)) return true;
        return false;
    }

    public bool hasItemOfType<T>()
    {
       foreach(var item in items) if (item.GetType() == typeof(T)) return true;
       return false;
    }

    public bool AddBattery(string batteryName, int currentPower, int id)
    {
        if(items.Count <= 0) return false;
        if(!items[selectedItemSlot].UsesBatteries) return false;
        if (items[selectedItemSlot].AddBattery(batteryName, currentPower, id))
        {
            for (int i = 0; i < batteryContainer.childCount; i++) 
                Destroy(batteryContainer.GetChild(i).gameObject);
            EnableBatteryUi(items[selectedItemSlot], batteryIndex);
            return true;
        }
        return false;
    }
}