using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameWindowsManager : MonoBehaviour {

    [SerializeField]
    private GameInput _gameInput;

    [SerializeField]
    private GameObject _gameWindowsBackground;

    [SerializeField]
    private GameObject _crosshair;

    [SerializeField]
    private UI_Inventory _UI_Inventory;

    [SerializeField]
    private vThirdPersonCamera _vThirdPersonCamera;

    private void Awake() {
        _gameWindowsBackground.SetActive(false);
    }

    private void Start() {
        _gameInput.OnOpenCloseInventoryAction += _gameInput_OnOpenCloseInventoryAction;                
    }

    private void _gameInput_OnOpenCloseInventoryAction(object sender, System.EventArgs e) {
        if(!_UI_Inventory.gameObject.activeSelf) { // show inventory
            _gameWindowsBackground.SetActive(true);
            _crosshair.SetActive(false);
            _UI_Inventory.Show();
            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            _vThirdPersonCamera.FullLock();
        } else { // hide inventory
            _gameWindowsBackground.SetActive(false);
            _crosshair.SetActive(true);
            _UI_Inventory.Hide();
            _vThirdPersonCamera.FullUnlock();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
