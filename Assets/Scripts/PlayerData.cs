using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerData : MonoBehaviour {

    private List<ItemDataContainer> _inventory;
    private ItemDataContainer _inventoryItemCache;

    private void Awake() {
        _inventory = new List<ItemDataContainer>();
    }

    public void AddItem(ItemDataContainer item) {
        Debug.Log("KB: _inventory.count: " + _inventory.Count);
        if (_inventory.Count > 0) {
            bool isItemRequiredToAdd = true;
            for (int i = 0; i < _inventory.Count; i++) {
                if (_inventory[i].ID == item.ID) {
                    isItemRequiredToAdd = false;
                    Debug.Log("KB: found item with ID: " + item.ID + " || current quantity: " + _inventory[i].Quantity);
                    // found item, increase quantity
                    _inventoryItemCache = _inventory[i];
                    _inventoryItemCache.Quantity += item.Quantity;
                    _inventory[i] = _inventoryItemCache;
                    Debug.Log("KB: increase quantity to: " + _inventoryItemCache.Quantity);
                    break;
                }
            }
            if (isItemRequiredToAdd) {
                // the list has items in it, but not this type of item, adding it to the list
                _inventory.Add(item);
                Debug.Log("KB: ID hasn't been found in initialized list, added item: " + item.Name);
            }
        } else {
            // nothing in the list yet
            _inventory.Add(item);
            Debug.Log("KB: No item in the list yet, adding item: " + item.Name);
        }
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.F)) {
            for (int i = 0; i < _inventory.Count; i++) {
                Debug.Log("KB: " + _inventory[i].Name + " | quantity: " + _inventory[i].Quantity);
            }
        }
    }
}
