using UnityEngine;
using UnityEngine.InputSystem;

// note:

// FIX JITTER:
// Prima la gerarchia era la seguente: Player > CameraHolder > camera
// per risolvere problemi di Jitter il cameraHolder e' stato posto "fuori dal player"
// e qui si controlla la posizione del cameraHolder, mentre la camera segue in modo fisso il cameraHolder
// dallo script CameraMove

// NO-FRICTION MATERIAL:
// E' stato aggiunto al Collider del player un physics material con friction = 0, in modo da evitare
// che esso rallenti quando sbatte con altri collider

public class PlayerController : MonoBehaviour {

    [SerializeField] private float speed;

    [SerializeField] Transform playerCameraHolder;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform orientation;

    private Rigidbody rb;
    private Vector3 movementDirection;

    private float cameraXRotation;
    private float cameraYRotation;

    public float GetCameraXRotation() => cameraXRotation;
    public float GetCameraYRotation() => cameraYRotation;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        if (Player.Instance.playerState == Player.PlayerState.Pause) {
            return; // In modo che il player non possa muovere la camera se e' in puzzle (in dialog si, quindi non lo metto qui)
        }

        // Gestione cursore
        UpdateCursor();

        if (Cursor.lockState == CursorLockMode.None)
            return;

        // Lettura input in update
        // Movimento dipende da dove sta guardando
        // gestisco rotazione player tramite orientation, quindi invece di transform.forward si usa orientation
        movementDirection = (orientation.forward * InputManager.Instance.GetMovement().z + orientation.right * InputManager.Instance.GetMovement().x).normalized;

        // Gestione movimento camera
        float mouseX = InputManager.Instance.GetMouse().x * mouseSensitivity * Time.deltaTime;
        float mouseY = InputManager.Instance.GetMouse().y * mouseSensitivity * Time.deltaTime;

        cameraYRotation += mouseX; // Rotazione della camera attorno ad y dipende da movimento lungo asse x del mouse
        cameraXRotation -= mouseY; // Movimento attorno ad x dipende invece da movimento del mouse lungo Y

        cameraXRotation = Mathf.Clamp(cameraXRotation, -90f, 90f); // La rotazione attorno ad asse x deve essere tra -90 e 90
        // in modo da evitare di piegare il "collo" all'indietro

        // si imposta rotazione della camera in base ai calcoli precedenti
        playerCameraHolder.transform.rotation = Quaternion.Euler(cameraXRotation, cameraYRotation, 0f);
        // si imposta orientamento del player in base a movimento X del mouse
        orientation.transform.rotation = Quaternion.Euler(0, cameraYRotation, 0);

        UpdateMovementState(); // Aggiornamento FSM
    }

    private void FixedUpdate() {
        if (rb == null) return;


        if (Player.Instance.playerState == Player.PlayerState.Pause ||
            Player.Instance.playerState == Player.PlayerState.Dialog) {
            movementDirection = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            return; // In modo che il player non possa muoversi se e' in puzzle o dialog
        }

        rb.linearVelocity = movementDirection * speed; // Per evitare jitter si muove con linearVelocity invece che con MovePosition
    }

    private void UpdateCursor() {
        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(1))
            Cursor.lockState = CursorLockMode.Locked;

        if (Cursor.lockState == CursorLockMode.Locked && Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
    }

    AudioSource source;
    private void UpdateMovementState() { // Aggiornamento FSM player
        if(movementDirection != Vector3.zero) {
            Player.Instance.playerState = Player.PlayerState.Movement;
            if(!source.isPlaying){ source.Play(); }
        }
        else {
            Player.Instance.playerState = Player.PlayerState.Idle;
            source.Stop();
        }
    }

    public void SetCameraRotation(float x, float y) {
        cameraXRotation = x;
        cameraYRotation = y;

        playerCameraHolder.rotation = Quaternion.Euler(cameraXRotation, cameraYRotation, 0f);
        orientation.rotation = Quaternion.Euler(0, cameraYRotation, 0);
    }
}
