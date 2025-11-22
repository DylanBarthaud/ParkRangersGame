using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject inventoryUi; 
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private int selectedItemSlot; 

    private void Awake()
    {
        inventoryUi.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) inventoryUi.SetActive(!inventoryUi.activeInHierarchy);
        if (Input.GetKeyDown(KeyCode.Mouse0) && items.Count > 0) items[selectedItemSlot].UseItem();
        if (Input.mouseScrollDelta.y > 0 && selectedItemSlot < items.Count - 1) selectedItemSlot++;
        if (Input.mouseScrollDelta.y < 0 && selectedItemSlot > 0) selectedItemSlot--;
    }

    public void AddItemToInventory(Item item)
    {
        items.Add(item);
    }
}
