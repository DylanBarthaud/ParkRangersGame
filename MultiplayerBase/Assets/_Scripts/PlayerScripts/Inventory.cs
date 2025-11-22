using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject inventoryUi;
    [SerializeField] GameObject[] inventorySlots; 
    private List<Item> items = new List<Item>();
    private int selectedItemSlot; 

    private void Awake()
    {
        inventorySlots[0].GetComponent<Image>().color = Color.green;
        inventoryUi.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) inventoryUi.SetActive(!inventoryUi.activeInHierarchy);
        if (Input.GetKeyDown(KeyCode.Mouse0) && items.Count > 0) items[selectedItemSlot].UseItem();
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
}
