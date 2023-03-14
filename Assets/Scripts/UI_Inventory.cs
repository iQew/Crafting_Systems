using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour {

    [SerializeField]
    private Transform _inventorySlotsGridTransform;

    [SerializeField]
    private UI_InventorySlot _inventorySlotPrefab;

    [SerializeField]
    private int _cols, _rows;

    [SerializeField]
    private int _slotSize;

    [SerializeField]
    private float _slotSpacing;

    private List<UI_InventorySlot> _inventorySlots;
    private UI_InventorySlot _inventorySlotCache;
    private RectTransform _inventorySlotRectTransform;

    private bool _initialized;

    private void Awake() {        
        _inventorySlots = new List<UI_InventorySlot>();       
    }

    private void Start() {
        /*for (int i = 0; i < 3; i++) {
			yield return null; // waiting for three frames
		}*/
        for (int j = 0; j < _rows; j++) {
            for (int k = 0; k < _cols; k++) {
                _inventorySlotCache = Instantiate(_inventorySlotPrefab);
                _inventorySlotCache.transform.SetParent(_inventorySlotsGridTransform, false);
                _inventorySlotCache.lastIndex = _rows * _cols - 1;
                _inventorySlotCache.index = k + j * _cols;
                _inventorySlotRectTransform = _inventorySlotCache.GetComponent<RectTransform>();
                _inventorySlotRectTransform.sizeDelta = Vector2.one * _slotSize;
                Vector2 position = new Vector2(
                    k * _slotSize + _slotSize / 2f,
                    j * -_slotSize - _slotSize / 2f);
                _inventorySlotRectTransform.anchoredPosition = position;
                _inventorySlotCache.name = "InventorySlot_ " + (k + j * _cols).ToString();
                _inventorySlots.Add(_inventorySlotCache);
            }
        }
        Hide();
    }

    public void Show() {        
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
