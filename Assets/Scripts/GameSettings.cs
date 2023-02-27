using UnityEngine;

public class GameSettings : MonoBehaviour {

    private bool _isCursorVisible = false;

    private void Awake() {
        Cursor.visible = _isCursorVisible;
    }

    private void Update() {
        if(Input.GetKeyUp(KeyCode.C)) {
            _isCursorVisible = !_isCursorVisible;
            Cursor.visible = _isCursorVisible;
        }
    }
}
