using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNotebookStartUp : MonoBehaviour {
    [SerializeField] private Transform notebookHolder; // GameObject che contiene notebook e camera relativa
    
    // Update is called once per frame
    void Update() {
        if (Player.Instance != null && Keyboard.current.iKey.wasPressedThisFrame && !Player.Instance.playerInteract.IsGrabbing()) {
            if (notebookHolder.gameObject.activeSelf) {
                notebookHolder.gameObject.SetActive(false);
                Player.Instance.playerState = Player.PlayerState.Idle;

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else {
                notebookHolder.gameObject.SetActive(true);
                Player.Instance.playerState = Player.PlayerState.Pause;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }    
    }
}
