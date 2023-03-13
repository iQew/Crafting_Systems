using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }

    [SerializeField]
    private GameInput _gameInput;

    [SerializeField]
    private InteractivityManager _interactivityManager;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError("There should only be one instance of " + name + ".");
        }
    }

    private void Start() {
        _gameInput.OnInteractAction += gameInput_OnInteractAction;
    }

    public bool PickupResource(Resource resource) {
        // TODO KB: Check if inventory is full
        if(Random.Range(0f,1f) > 0.5f) {
            Debug.Log("KB: added item: " + resource.ItemDataContainer.Name
                + " | quantity: " + resource.ItemDataContainer.Quantity
                + " | ID: " + resource.ItemDataContainer.ID);
            return true;
        }
        return false;
    }

    private void gameInput_OnInteractAction(object sender, System.EventArgs e) {
        _interactivityManager.PickUpActiveResource();
    }
}
