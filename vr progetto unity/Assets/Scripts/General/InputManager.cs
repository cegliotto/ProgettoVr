using UnityEngine;

public class InputManager : MonoBehaviour {
    
    public static InputManager Instance;
    private InputSystem_Actions inputActions;

    private Vector3 movement;
    private Vector2 look;

    public Vector3 GetMovement() => movement;
    public Vector2 GetMouse() => look;

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

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        movement = new Vector3(obj.ReadValue<Vector2>().x, 0f, obj.ReadValue<Vector2>().y);
    }

    private void Look_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        look = obj.ReadValue<Vector2>();
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
