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

    private void Awake()
    {
        inventorySlots[0].GetComponent<Image>().color = Color.green;
        inventoryUi.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) inventoryUi.SetActive(!inventoryUi.activeInHierarchy);

        if (Input.GetKeyDown(KeyCode.Mouse0) 
            && items.Count > 0 
            && !IsItemNull(items[selectedItemSlot])) 
        {
            Item selectedItem = items[selectedItemSlot];
            selectedItem.UseItem(gameObject);
            if(selectedItem.HasAudio) GetComponent<AudioHandler>().PlaySoundClientRpc(selectedItem.AudioName, default, selectedItem.AudioVolume);
            if (!selectedItem.InfiniteUses && selectedItem.Uses <= 0) RemoveItem(selectedItem); 
        }

        if (Input.GetKeyDown(KeyCode.Q) && items.Count > 0)
        {
            if (IsItemNull(items[selectedItemSlot])) return; 
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

    private bool IsItemNull(Item item)
    {
        if (item == null) return true;
        return false; 
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
