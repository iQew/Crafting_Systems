using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {

    public event EventHandler OnInteractAction;
    public event EventHandler OnOpenCloseInventoryAction;

    private PlayerInputActions _playerInputActions;

    private void Awake() {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.Interact.performed += Interact_performed;
        _playerInputActions.Player.OpenCloseInventory.performed += ShowInventory_performed;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void ShowInventory_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnOpenCloseInventoryAction?.Invoke(obj, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
}
