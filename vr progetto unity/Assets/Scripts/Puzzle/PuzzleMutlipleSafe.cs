using UnityEngine;

public class PuzzleMutlipleSafe : PuzzleBase {

    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private SafeDial[] dials;
    [SerializeField] private int[] valuesToGuess = { 5, 10, 15 };

    private SafeDial activeDial;
    private bool solved;

    private bool isDragging;
    private Vector3 lastMousePos;

    void Update() {
        PuzzleBehaviour();
    }

    protected override void PuzzleBehaviour() {
        if (solved)
            return;

        SelectDial();
        HandleInput();
        CheckCombination();
    }

    private void SelectDial() {
        if (isDragging) return; // IMPORTANT

        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            activeDial = hit.collider.GetComponent<SafeDial>();
        else
            activeDial = null;
    }


    [SerializeField] private float mouseToDegrees = 0.25f; // sensibilità
    [SerializeField] private float deadZonePx = 0.5f;      // ignora jitter
    [SerializeField] private float maxDegreesPerFrame = 8f;

    private void HandleInput() {
        // Inizia drag solo se ho una dial sotto
        if (Input.GetMouseButtonDown(0)) {
            if (activeDial == null) return;

            isDragging = true;
            lastMousePos = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButtonUp(0)) {
            isDragging = false;
            return;
        }

        if (!isDragging || activeDial == null)
            return;

        float deltaX = Input.mousePosition.x - lastMousePos.x;

        // deadzone anti-jitter
        if (Mathf.Abs(deltaX) < deadZonePx) {
            lastMousePos = Input.mousePosition;
            return;
        }

        float rotationInput = deltaX * mouseToDegrees;

        // clamp anti-salti grossi
        rotationInput = Mathf.Clamp(rotationInput, -maxDegreesPerFrame, maxDegreesPerFrame);

        activeDial.Rotate(rotationInput);

        lastMousePos = Input.mousePosition;

        LogCurrentCombination();
    }


    private void CheckCombination() {
        for (int i = 0; i < valuesToGuess.Length; i++) {
            if (dials[i].CurrentValue != valuesToGuess[i])
                return;
        }

        solved = true;
        PuzzleCompleted();
    }

    protected override void PuzzleCompleted() {
        Debug.Log("Cassaforte aperta!");

        if (PuzzleManager.Instance != null) {
            PuzzleManager.Instance.CompletePuzzle();
        }
    }

    private void LogCurrentCombination() {
        string combo = "";
        foreach (var dial in dials) {
            combo += dial.CurrentValue + " ";
        }
        Debug.Log($"Combinazione attuale: {combo}");
    }
}
