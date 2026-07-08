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
    [SerializeField] GameObject inventoryUi;
    [SerializeField] GameObject[] inventorySlots;
    [SerializeField] Sprite baseInvSlotSprite; 
    [SerializeField] private List<Item> items = new List<Item>();
    public List<Item> Items => items;
    [SerializeField] private int selectedItemSlot;
    public int SelectedItemSlot => selectedItemSlot;

    [HideInInspector] public bool canUseInv = true;

    [SerializeField] private GameObject carryingHeavyGFX;
    [SerializeField] private Transform heldItemPos;
    private bool carryingHeavy = false;
    public bool CarryingHeavy => carryingHeavy;
    private Item heavyItem = null;
    public Item HeavyItem => heavyItem;

    private void Awake()
    {
        inventorySlots[0].GetComponent<Image>().color = Color.green;
        inventoryUi.SetActive(false);

        EventManager.instance.onPuzzleComplete += OnPuzzleComplete;
        EventManager.instance.onButtonReleased += EnableInv;
        EventManager.instance.onButtonHeld += OnButtonHeld; 

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
            && items.Count > 0)
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
                EnableCarriedItemGFX(selectedItem, false);
                RemoveItem(selectedItem);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && items.Count > 0)
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

        if(carryingHeavy) return;

        if (Input.mouseScrollDelta.y > 0 && selectedItemSlot < items.Count - 1)
        {
            EnableCarriedItemGFX(items[selectedItemSlot], false);
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.gray;
            selectedItemSlot++;
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.green;
            EnableCarriedItemGFX(items[selectedItemSlot], true);
        }

        if (Input.mouseScrollDelta.y < 0 && selectedItemSlot > 0)
        {
            EnableCarriedItemGFX(items[selectedItemSlot], false);
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.gray;
            selectedItemSlot--;
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.green;
            EnableCarriedItemGFX(items[selectedItemSlot], true);
        }
    }

    public bool CanAddItemToInv()
    {
        if (items.Count >= inventorySlots.Length) return false;
        return true; 
    }

    public void AddItemToInventory(Item item)
    {
        item.GFXHandler.ChangeGFXRenderLayerServerRpc("ItemGFX", 4);
        if(items.Count <= 0 || item.IsHeavy) EnableCarriedItemGFX(item, true);

        if (item.IsHeavy)
        {
            carryingHeavy = true; 
            heavyItem = item;
            carryingHeavyGFX.SetActive(true);

            EnableCarriedItemGFX(items[selectedItemSlot], false);

            return;
        }

        items.Add(item);
        inventorySlots[items.Count - 1].transform.GetChild(0).GetComponent<Image>().sprite = item.Sprite;
    }

    public void RemoveItem(Item item)
    {
        item.isBeingHeld = false;
        item.GFXHandler.ChangeGFXRenderLayerServerRpc("ItemGFX", 2);

        if (item.IsHeavy)
        {
            carryingHeavy = false;
            heavyItem = null;
            carryingHeavyGFX.SetActive(false);

            EnableCarriedItemGFX(items[selectedItemSlot], true);

            return;
        }

        inventorySlots[items.Count - 1].transform.GetChild(0).GetComponent<Image>().sprite = baseInvSlotSprite;
        if (selectedItemSlot > 0)
        {
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.gray;
            selectedItemSlot--;
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.green;
            EnableCarriedItemGFX(items[selectedItemSlot], true);
        }

        items.Remove(item);

        int i = 0;
        foreach (var currentItem in items)
        {
            inventorySlots[i].transform.GetChild(0).GetComponent<Image>().sprite = currentItem.Sprite;
            i++;
        }

    }

    private void EnableCarriedItemGFX(Item item, bool enable)
    {
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
}