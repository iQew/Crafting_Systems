using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory {
    
    private int _cols, _rows;
    private int _size;
    private ItemDataSO _itemCache;

    public List<ItemDataSO> ItemDataList { get; private set; }
    public List<int> ItemQuantityList { get; private set; }

    public Inventory(int cols, int rows) {
        _cols = cols;
        _rows = rows;
        _size = cols * rows;
        ItemDataList = new List<ItemDataSO>();
        ItemQuantityList = new List<int>();
    }

    public bool AddItem(ItemDataSO item, int quantity) {
        int count = ItemDataList.Count;
        if(ItemDataList.Count == 0) {
            // nothing in the list yet
            ItemDataList.Add(item);
            ItemQuantityList.Add(quantity);
            Debug.Log("KB: No item in the list yet, adding item: " + item.Name);
            return true;
        } else {
            bool isItemAdded = false;
            for (int i = 0; i < count; i++) {
                if (ItemDataList[i].ID == item.ID) {
                    if (ItemQuantityList[i] + quantity <= ItemDataList[i].StackSize) {
                        ItemQuantityList[i] += quantity;
                        isItemAdded = true;
                        Debug.Log("KB: " + item.name + " | was added to stack at index: " + i);
                        return true;
                    }
                    break;
                }
            }
            if (!isItemAdded) { 
                if(ItemDataList.Count == _size) {
                    // no more space in inventory
                    Debug.Log("KB: no more space in inventory.");
                    return false;
                } else {
                    ItemDataList.Add(item);
                    ItemQuantityList.Add(quantity);
                    Debug.Log("KB: found new empty slot for item: " + item.name + " with quantity: " + quantity);
                    return true;
                }
            }
            return false;
        }
    }
}
