using UnityEngine;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(MeshRenderer))]
public class CollectibleResource : MonoBehaviour {

    [HideInInspector]
    public float Proximity { get; set; } // used for the player to determine which resource is closest

    private bool _active;
    public bool Active {
        private get {
            return _active;
        }
        set {
            _material.SetFloat("_IsSelected", value ? 1f : 0f);
            _active = value;
        }
    }

    [SerializeField]
    [Tooltip("How many resources are looted MIN and MAX.")]
    private Vector2 _dropQuantityMinMax = new Vector2(1f, 1f);

    public int Quantity { get => Mathf.RoundToInt(Random.Range(_dropQuantityMinMax.x, _dropQuantityMinMax.y)); private set { } }

    [SerializeField]
    private ItemDataSO _itemData;

    private MeshRenderer _meshRenderer;
    private Material _material;

    private void Awake() {
        if (_itemData == null) {
            Debug.LogError("Missing InventoryItem");
        } else if (_itemData.ID == 0) {
            Debug.LogError(_itemData.Name + " has not set its ID");
        }
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = _meshRenderer.material;
    }

    public void SetActive(bool active) {
        _material.SetFloat("_IsSelected", active ? 1f : 0f);
    }

    public void PickUp() {
       Destroy(gameObject);
    }

    public ItemDataSO GetItemDataSO() {
        return _itemData;
    }
}
