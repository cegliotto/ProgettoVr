using UnityEngine;

public class PuzzleCabinet : PuzzleBase {

    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private SafeDial[] dials;
    [SerializeField] private int[] valuesToGuess = { 5, 10, 15 }; // Valori da indovinare

    [SerializeField] private float mouseToDegrees = 0.15f; // sensibilità
    [SerializeField] private float deadZonePx = 0.5f;      // Per evitare jitter
    [SerializeField] private float maxDegreesPerFrame = 8f;

    private SafeDial activeDial;

    private bool isDragging;
    private Vector3 lastMousePos;

    void Update() {
        PuzzleBehaviour();

        ExitPuzzle();
    }

    protected override void PuzzleBehaviour() {
        SelectDial();
        HandleInput();
        CheckCombination();
    }


    protected override void ExitPuzzle() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (PuzzleManager.Instance != null) {
                PuzzleManager.Instance.ExitFromPuzzle();
            }
        }
    }

    private void SelectDial() {
        if (isDragging) return; // Se sto gia' draggando un dial, non voglio selezionarne uno nuovo

        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition); // raycast per il selezionamento del dial

        float raycastDistance = 5f;
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            activeDial = hit.collider.GetComponent<SafeDial>();
        else
            activeDial = null;
    }

    private void HandleInput() {
        // Il drag inizia solo se si ha un dial sotto il mouse
        if (Input.GetMouseButtonDown(0)) {
            if (activeDial == null) return;

            isDragging = true;
            lastMousePos = Input.mousePosition;
            return;
        }

        // Quando si smette di premere il bottone del mouse il drag si toglie
        if (Input.GetMouseButtonUp(0)) {
            isDragging = false;
            return;
        }

        if (!isDragging || activeDial == null)
            return;
        // Rotazione dei dials avviane per spostamento orizzontale del mouse
        float deltaX = Input.mousePosition.x - lastMousePos.x;

        // Per evitare jitter
        if (Mathf.Abs(deltaX) < deadZonePx) {
            lastMousePos = Input.mousePosition;
            return;
        }

        float rotationInput = deltaX * mouseToDegrees;
        // clamp anti-salti grossi
        rotationInput = Mathf.Clamp(rotationInput, -maxDegreesPerFrame, maxDegreesPerFrame);
        activeDial.Rotate(rotationInput);
        lastMousePos = Input.mousePosition;

        //LogCurrentCombination(); // debug per capire se cifre sono corrette
    }

    private void CheckCombination() {
        // Se la combinazione inserita (valore corrente di ogni dials) combacia con quella richiesta
        for (int i = 0; i < valuesToGuess.Length; i++) {
            if (dials[i].CurrentValue != valuesToGuess[i])
                return;
        }
        // Allora si esegue il metodo in PuzzleManager
        PuzzleCompleted();
    }

    protected override void PuzzleCompleted() {
        Debug.Log("Puzzle completato!");

        if (PuzzleManager.Instance != null) {
            PuzzleManager.Instance.CompletePuzzle();
        }
    }

    private void LogCurrentCombination() {
        string combo = "";
        foreach (SafeDial dial in dials) {
            combo += dial.CurrentValue + " ";
        }
        Debug.Log($"Combinazione attuale: {combo}");
    }
}
