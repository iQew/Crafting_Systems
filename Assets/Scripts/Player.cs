using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }

    [SerializeField]
    private GameInput _gameInput;

    [SerializeField]
    private InteractivityManager _interactivityManager;

    private ItemDataContainer _itemDataContainerCache;

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

    private void gameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (_interactivityManager.TryPickUpItem(out _itemDataContainerCache)) {
            Debug.Log(
                "KB: picking up item: " + _itemDataContainerCache.Name
                + " | amount: " + _itemDataContainerCache.Quantity
                + " | ID: " + _itemDataContainerCache.ID
                );
        } else {
            Debug.Log("KB: No item to pickup.");
        }
    }
}
