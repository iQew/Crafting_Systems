using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour {
        
    
    [SerializeField]
    private UI_InventorySlot _inventorySlotPrefab;

    [SerializeField]
    private RectTransform _parentRectTransform;

    [SerializeField]
    private int _cols, _rows;

    [SerializeField]
    private int _slotSize;

    [SerializeField]
    private float _spacing;

    private List<UI_InventorySlot> _inventorySlots;
    private UI_InventorySlot _inventorySlotCache;
    private Transform _inventorySlotsGridTransform;
    private RectTransform _inventorySlotRectTransform;
        

    private bool _initialized;

    private void Awake() {        
        _inventorySlots = new List<UI_InventorySlot>();
        _inventorySlotsGridTransform = _parentRectTransform.transform;
    }

    private void Start() {
        for (int j = 0; j < _rows; j++) {
            for (int k = 0; k < _cols; k++) {
                _inventorySlotCache = Instantiate(_inventorySlotPrefab);
                _inventorySlotCache.transform.SetParent(_inventorySlotsGridTransform, false);
                _inventorySlotCache.lastIndex = _rows * _cols - 1;
                _inventorySlotCache.index = k + j * _cols;
                _inventorySlotRectTransform = _inventorySlotCache.GetComponent<RectTransform>();
                _inventorySlotRectTransform.sizeDelta = Vector2.one * _slotSize;
                Vector2 padding = new Vector2(_spacing * k, _spacing * j);
                // calculating offsets for slots that have top left alignment (need to offset to the right and down)
                float centeringOffsetX = (_parentRectTransform.rect.size.x - (_cols * _slotSize + _spacing * (_cols - 1))) / 2f;
                float centeringOffsetY = (_parentRectTransform.rect.size.y - (_rows * _slotSize + _spacing * (_rows - 1))) / 2f;
                Vector2 position = new Vector2(
                    k * _slotSize + _slotSize / 2f + padding.x + centeringOffsetX,
                    j * -_slotSize - _slotSize / 2f - padding.y - centeringOffsetY);
                _inventorySlotRectTransform.anchoredPosition = position;
                _inventorySlotCache.name = "InventorySlot_ " + (k + j * _cols).ToString();
                _inventorySlots.Add(_inventorySlotCache);
            }
        }
        //Hide();
    }

    public void Show() {        
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
