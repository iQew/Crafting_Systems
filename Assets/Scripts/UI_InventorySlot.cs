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

    [SerializeField]
    private GameObject _highlight;

    [SerializeField]
    private float _maxScaling = 1.125f;

    [SerializeField]
    private float _animationTime = 0.175f;

    private RectTransform _rectTransform;
    private Vector2 _defaultSize;
    private Vector2 _currentSize;

    private bool _isAnimating;
    private float _targetSize;    
    private float _animationStep;
    private bool _isSelected;
    private float _previousAnimationProgress;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();        
        ShowEmpty();
    }

    private void Start() {
        _defaultSize = _rectTransform.sizeDelta;
    }

    private void Update() {
        if(_isAnimating) {
            if(_isSelected) {
                _rectTransform.sizeDelta = Vector2.Lerp(_currentSize, _defaultSize * _targetSize, _animationStep);
            } else {
                _rectTransform.sizeDelta = Vector2.Lerp(_currentSize, _defaultSize, (float)Tweening.EaseOutExpo((double)_animationStep));
            }
            _animationStep += Time.deltaTime / _animationTime / _previousAnimationProgress;
            if(_animationStep >= 1f) {
                _animationStep = 1f;
                _isAnimating = false;                
            }
        }
    }    

    public void UpdateItem(InventoryItem item, int quantity) {
        if (item != null) {
            _itemImage.sprite = item.artwork;
            _quantityText.text = StringHelper.GetLeadingZeroNumberString(quantity);
            _itemImageGameObject.SetActive(true);
        }
    }

    public void ShowEmpty() {
        _itemImageGameObject.SetActive(false);
    }

    public void Select() {
        _isSelected = true;
        _currentSize = _rectTransform.sizeDelta;
        _targetSize = _maxScaling;
        transform.SetSiblingIndex(lastIndex);
        _highlight.SetActive(true);
        _previousAnimationProgress = _animationStep == 0 ? 1f : _animationStep;
        _animationStep = 0f;
        _isAnimating = true;
    }

    public void Unselect() {
        _isSelected = false;
        _currentSize = _rectTransform.sizeDelta;
        transform.SetSiblingIndex(index);
        _highlight.SetActive(false);
        _previousAnimationProgress = _animationStep == 0 ? 1f : _animationStep;
        _animationStep = 0f;
        _isAnimating = true;
    }

    private void OnEnable() {
        Unselect();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Select();
    }

    public void OnPointerExit(PointerEventData eventData) {
        Unselect();
    }
}
