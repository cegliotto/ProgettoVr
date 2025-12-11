using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] private float speed;

    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity;

    private Rigidbody rb;
    private Vector3 movementDirection;

    private float cameraXRotation;

    private void Awake() {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        // Gestione cursore
        UpdateCursor();

        if (Cursor.lockState == CursorLockMode.None)
            return;

        // Lettura input in update
        // Movimento dipende da dove sta guardando
        movementDirection = (transform.forward * InputManager.Instance.GetMovement().z + transform.right * InputManager.Instance.GetMovement().x).normalized;

        // Gestione movimento camera
        float mouseX = InputManager.Instance.GetMouse().x * mouseSensitivity * Time.deltaTime;
        float mouseY = InputManager.Instance.GetMouse().y * mouseSensitivity * Time.deltaTime;
        
        cameraXRotation -= mouseY;
        float cameraRotationClamp = 90f; // Dopo quanto si deve fermare lo spostamento verticale
        cameraXRotation = Mathf.Clamp(cameraXRotation, -cameraRotationClamp, cameraRotationClamp);
        playerCamera.localRotation = Quaternion.Euler(cameraXRotation, 0f, 0f);

        transform.Rotate(Vector3.up, mouseX); // Rotazione player sulla base di mouseX attorno ad asse y
    }

    private void FixedUpdate() {
        if (rb == null) return;

        rb.MovePosition(rb.position + movementDirection * speed * Time.fixedDeltaTime);
    }

    private void UpdateCursor() {
        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(1))
            Cursor.lockState = CursorLockMode.Locked;

        if (Cursor.lockState == CursorLockMode.Locked && Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
    }
}
