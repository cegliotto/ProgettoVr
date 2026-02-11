using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    
    public static InputManager Instance;
    private InputSystem_Actions inputActions;

    private Vector3 movement;
    private Vector2 look;

    public Vector3 GetMovement() {
        return inputEnabled ? movement : Vector3.zero;
    }

    public Vector2 GetMouse() {
        return inputEnabled ? look : Vector2.zero;
    }

    private bool inputEnabled = true;

    public void SetInputEnabled(bool enabled) {
        inputEnabled = enabled;

        if (!enabled) {
            movement = Vector3.zero;
            look = Vector2.zero;
        }
    }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new InputSystem_Actions();

        inputActions.Player.Enable(); // Inizializzazione sistema di input relativo al Player

        // Movimento
        inputActions.Player.Move.performed += Move_performed;
        inputActions.Player.Move.canceled += Move_canceled;
        // Sguardo
        inputActions.Player.Look.performed += Look_performed;
        inputActions.Player.Look.canceled += Look_canceled;

        // Interazione
        inputActions.Player.Interact.performed += Interact_performed;
    }

    private void Move_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        movement = Vector3.zero;
    }

    private void Look_performed(InputAction.CallbackContext obj) {
        if (!inputEnabled) return;
        look = obj.ReadValue<Vector2>();
    }

    private void Move_performed(InputAction.CallbackContext obj) {
        if (!inputEnabled) return;
        movement = new Vector3(obj.ReadValue<Vector2>().x, 0f, obj.ReadValue<Vector2>().y);
    }

    private void Look_canceled(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        look = Vector2.zero;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if(Player.Instance != null){
            Player.Instance.playerInteract.Interact();} // Richiamo metodo di interazione del player
    }


    private void OnDisable() {
        inputActions.Player.Disable();
    }

    private void OnDestroy() {
        if (inputActions != null) {
            inputActions.Player.Move.performed -= Move_performed;
            inputActions.Player.Move.canceled -= Move_canceled;

            inputActions.Player.Look.performed -= Look_performed;
            inputActions.Player.Look.canceled -= Look_canceled;

            inputActions.Player.Interact.performed -= Interact_performed;

            inputActions.Player.Disable();
        }
    }
}
