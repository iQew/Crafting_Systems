using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Resource : MonoBehaviour {   

    public IResourceReceiver ResourceReceiver { private get; set; }

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

    [SerializeField]
    private int _health = 5;

    [SerializeField]
    private InventoryItem _inventoryItem;

    private MeshRenderer _meshRenderer;
    private Material _material;

    private bool _markedForDeath;

    private void Awake() {        
        if(_inventoryItem == null) {
            Debug.LogError("Missing InventoryItem");
        } else if(_inventoryItem.ID == 0) {
            Debug.LogError(_inventoryItem.name + " has not set its ID");
        }
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = _meshRenderer.material;
    }

    private void Update() {
        if(_active) {
            if(Input.GetKeyUp(KeyCode.E)) {
                _health--;
                Debug.Log("KB: " + name + " received interaction key, decreasing health.");
                if(_health <= 0) {
                    ResourceReceiver.OnResourceDestruction(new ItemDataContainer(
                        _inventoryItem.ID,
                        _inventoryItem.name,
                        Random.Range((int)_dropQuantityMinMax.x, (int)_dropQuantityMinMax.y)
                     ));
                    Destroy(gameObject);
                }
            }
        }        
    }
    

    public void SetActive(bool active) {        
        _material.SetFloat("_IsSelected", active ? 1f : 0f);
    }
}
