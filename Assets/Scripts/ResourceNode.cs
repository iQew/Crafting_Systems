using UnityEngine;

public class ResourceNode : MonoBehaviour {   

    public IResourceReceiver ResourceReceiver { private get; set; }

    [SerializeField]
    private int health = 5;

    [SerializeField]
    private InventoryItem _inventoryItem;

    private void Awake() {        
        if(_inventoryItem == null) {
            Debug.LogError("Missing InventoryItem");
        } else if(_inventoryItem.ID == 0) {
            Debug.LogError(_inventoryItem.name + " has not set its ID");
        }        
    }

    private void Update() {
        if(Input.GetKeyUp(KeyCode.E)) {
            ResourceReceiver.OnResourceDestruction(new ItemDataContainer(
                _inventoryItem.ID,
                _inventoryItem.name,
                Random.Range(1, 5)
            ));
        }
    }
}
