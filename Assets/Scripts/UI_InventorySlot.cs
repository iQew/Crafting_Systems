using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UI_InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    
    public int index { private get; set; }
    public int lastIndex { private get; set; }

    [SerializeField]
    private GameObject _itemImageGameObject;

    [SerializeField]
    private Image _itemImage;

    [SerializeField]
    private TextMeshProUGUI _quantityText;

    private RectTransform _rectTransform;
    private Vector3 _defaultSize;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();        
        ShowEmpty();
    }

    private IEnumerator Start() {
        for (int i = 0; i < 3; i++) { // wait for 3 frames to finish sizing
            yield return null;
        }
        _defaultSize = _rectTransform.sizeDelta;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Select();
    }

    public void OnPointerExit(PointerEventData eventData) {
        Unselect();
    }

    public void UpdateItem(InventoryItem item, int quantity) {
        if (item != null) {
            _itemImage.sprite = item.artwork;
            _quantityText.text = GetLeadingZeroNumberString(quantity);
            _itemImageGameObject.SetActive(true);
        }
    }

    public void ShowEmpty() {
        _itemImageGameObject.SetActive(false);
    }

    public void Select() {
        Debug.Log("KB: selected slot: " + name);
        _rectTransform.sizeDelta *= Vector3.one * 1.15f;
        transform.SetSiblingIndex(lastIndex);
        Debug.Log("KB: setting: " + name + " to last index: " + lastIndex);
    }

    public void Unselect() {
        Debug.Log("KB: unselected slot: " + name);
        _rectTransform.sizeDelta = _defaultSize;
        transform.SetSiblingIndex(index);
        Debug.Log("KB: setting: " + name + " to original index: " + index);
    }

    private string GetLeadingZeroNumberString(int number) {
        string result = number.ToString();

        if (number < 10) {
            result = "0" + number.ToString();
        }
        return result;
    }

}
