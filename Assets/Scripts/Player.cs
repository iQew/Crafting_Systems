using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }

    [SerializeField]
    private GameInput _gameInput;

    [SerializeField]
    private InteractivityManager _interactivityManager;

    [SerializeField]
    private UI_Inventory _UI_Inventory;

    [SerializeField]
    private UI_LootInfoBox _UI_LootInfoBox;

    private Inventory _inventory;    

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError("There should only be one instance of " + name + ".");
        }
        _inventory = new Inventory(6, 5);        
    }

    private void Start() {
        _gameInput.OnInteractAction += gameInput_OnInteractAction;
    }

    public bool PickupResource(CollectibleResource resource) {
        bool itemAdded = _inventory.AddItem(resource.GetItemDataSO(), resource.Quantity);
        if (itemAdded) {
            Vector2 XP_Data = PlayerStats.Instance.AddExperienceGatheringForaging(resource.GetItemDataSO());
            _UI_LootInfoBox.Show(PlayerStats.ExperienceType.GATHERING_FORAGING, (int)XP_Data.x, (int)XP_Data.y);
            _UI_Inventory.Refresh(_inventory);
            return true;
        }
        return false;
    }

    private void gameInput_OnInteractAction(object sender, System.EventArgs e) {
        _interactivityManager.PickUpActiveResource();
    }
}
