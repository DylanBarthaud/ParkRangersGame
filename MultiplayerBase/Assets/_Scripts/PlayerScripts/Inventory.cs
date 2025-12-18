using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject inventoryUi;
    [SerializeField] GameObject[] inventorySlots;
    [SerializeField] Sprite baseInvSlotSprite; 
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private int selectedItemSlot;

    [HideInInspector] public bool canUseInv = true;

    private void Awake()
    {
        inventorySlots[0].GetComponent<Image>().color = Color.green;
        inventoryUi.SetActive(false);

        EventManager.instance.onPuzzleComplete += OnPuzzleComplete;
        EventManager.instance.onButtonReleased += EnableInv;
        EventManager.instance.onButtonHeld += OnButtonHeld; 

    }

    private void OnButtonHeld(int arg1, Interactor interactor)
    {
        DisableInv();
    }

    private void OnPuzzleComplete(bool arg1, IInteractable interactable)
    {
        EnableInv();
    }

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
        if (Input.GetKeyDown(KeyCode.Tab)) inventoryUi.SetActive(!inventoryUi.activeInHierarchy);

        if (Input.GetKeyDown(KeyCode.Mouse0) 
            && items.Count > 0 
            && items[selectedItemSlot] != null) 
        {
            Item selectedItem = items[selectedItemSlot];
            selectedItem.UseItem(gameObject);
            if(selectedItem.HasAudio) GetComponent<MultiplayerAudioHandlerWrapper>().PlaySoundServerRpc(selectedItem.AudioName, default, selectedItem.AudioVolume);
            if (!selectedItem.InfiniteUses && selectedItem.Uses <= 0) RemoveItem(selectedItem); 
        }

        if (Input.GetKeyDown(KeyCode.Q) && items.Count > 0)
        {
            if (items[selectedItemSlot] == null) return; 
            Vector3 newPos = new Vector3(transform.position.x + 4, transform.position.y, transform.position.z);
            items[selectedItemSlot].DropItem(newPos);
            RemoveItem(items[selectedItemSlot]);
        }

        if (Input.mouseScrollDelta.y > 0 && selectedItemSlot < items.Count - 1)
        {
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.gray;
            selectedItemSlot++;
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.green;
        }

        if (Input.mouseScrollDelta.y < 0 && selectedItemSlot > 0)
        {
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.gray;
            selectedItemSlot--;
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.green;
        }
    }

    public bool AddItemToInventory(Item item)
    {
        if(items.Count >= inventorySlots.Length) return false;

        items.Add(item);
        inventorySlots[items.Count - 1].transform.GetChild(0).GetComponent<Image>().sprite = item.Sprite;
        return true;
    }

    public void RemoveItem(Item item)
    {
        inventorySlots[items.Count - 1].transform.GetChild(0).GetComponent<Image>().sprite = baseInvSlotSprite;
        if (selectedItemSlot > items.Count - 1 && selectedItemSlot > 0)
        {
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.gray;
            selectedItemSlot--;
            inventorySlots[selectedItemSlot].GetComponent<Image>().color = Color.green;

        }

        items.Remove(item);

        int i = 0;
        foreach (var currentItem in items)
        {
            inventorySlots[i].transform.GetChild(0).GetComponent<Image>().sprite = currentItem.Sprite;
            i++;
        }

    }
}