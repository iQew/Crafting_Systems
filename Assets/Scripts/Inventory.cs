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

    private List<int> _availableStacksIndices;

    public Inventory(int cols, int rows) {
        _cols = cols;
        _rows = rows;
        _size = cols * rows;
        ItemDataList = new List<ItemDataSO>();
        ItemQuantityList = new List<int>();
        _availableStacksIndices = new List<int>();
    }

    public bool AddItem(ItemDataSO item, int quantity) {
        Debug.Assert(item.StackSize >= quantity);
        Debug.Log("KB: trying to pickup " + item.name + " x" + quantity);
        int count = ItemDataList.Count;
        if (ItemDataList.Count >= _size) {
            // no more space in inventory
            Debug.Log("KB: no more space in inventory.");
            return false;
        } else if (ItemDataList.Count == 0) {
            // nothing in the list yet
            ItemDataList.Add(item);
            ItemQuantityList.Add(quantity);
            Debug.Log("KB: No item in the list yet, adding item: " + item.Name);
            return true;
        } else {
            _availableStacksIndices.Clear();
            for (int i = 0; i < count; i++) {
                if (ItemDataList[i].ID == item.ID) {
                    if (ItemQuantityList[i] + quantity <= ItemDataList[i].StackSize) {
                        ItemQuantityList[i] += quantity;
                        Debug.Log("KB: " + item.name + " was added to stack at index: " + i);
                        return true;
                    } else {

                        int maxStackSize = ItemDataList[i].StackSize;
                        int currentSize = ItemQuantityList[i];
                        int availableSpace = maxStackSize - currentSize;

                        if (availableSpace > 0) {
                            ItemQuantityList[i] += availableSpace;
                            quantity -= availableSpace;

                            Debug.Log("KB: found stack of same ID, increase capacity to: " + availableSpace + " at index: " + i);
                        }
                    }
                }
            }
            if (quantity > 0) {
                ItemDataList.Add(item);
                ItemQuantityList.Add(quantity);
                return true;
            } else {
                return false;
            }
        }
    }
}
