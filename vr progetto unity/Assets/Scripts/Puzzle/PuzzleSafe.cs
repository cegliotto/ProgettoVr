using UnityEngine;

public class PuzzleSafe : PuzzleBase
{
    [SerializeField] private bool debug;
    [SerializeField] private Transform combinationTransform; // Ghiera
    [SerializeField] private int[] valuesToGuess = { 5, 10, 15 }; // Numeri da inserire per indovinare

    [SerializeField] private float rotationPerStep = 9f; // Rotazione necessaria per passare a numero successivo
    private int currentDigit; // Counter per capire quale in quale numero nella ghiera siamo
    [SerializeField] private float rotationRepeatDelay = 0.15f;
    private float lastRotationTime;

    [SerializeField] private int[] currentCombinationInserted; // Combinazione attualmente inserita
    private int currentGuessedDigit; // cifre attualmente inserite in currentCombinationInserted
    private void Awake() {
        currentCombinationInserted = new int[valuesToGuess.Length];
    }

    private void Update() {
        PuzzleBehaviour();
    }

    protected override void PuzzleBehaviour() {
        // Ruota la ghiera
        RotateCombinationSafe();
        // "Inserisce" numero attuale della ghiera
        GuessDigit();

        // mostrare in UI i numeri scelti e i comandi (magari da pannello info
    }

    protected override void PuzzleCompleted() {
        // Si esegue metodo CompletePuzzle in PuzzleManager -> passare item corretto
        if(PuzzleManager.Instance != null) {
            PuzzleManager.Instance.CompletePuzzle();
        }
    }

    private void RotateCombinationSafe() {
        if (Time.time - lastRotationTime < rotationRepeatDelay)
            return;

        if (Input.GetKey(KeyCode.D)) {
            combinationTransform.Rotate(0, 0, -rotationPerStep, Space.Self);
            // contatore avanti -> arrivato a 40 torna a 0
            GetCombinationNumber(1);
            lastRotationTime = Time.time;
        }
        else if (Input.GetKey(KeyCode.A)) {
            combinationTransform.Rotate(0, 0, rotationPerStep, Space.Self);
            // contatore indietro, se va a -1 diventa 39
            GetCombinationNumber(-1);
            lastRotationTime = Time.time;
        }
    }

    private void GuessDigit() {
        if (Input.GetMouseButtonDown(0)) { // Alla pressione del tasto sinistro del mouse
            currentCombinationInserted[currentGuessedDigit] = currentDigit;
            if (currentGuessedDigit >= valuesToGuess.Length - 1) { // Se ho inserito 3 digits
                if (CompareGuessedWithValues()) { // Se i due array combaciano
                    if (debug) Debug.Log("Combinazione giusta!");
                    PuzzleCompleted(); // puzzle completato
                }

                // se ho sbagliato finisco qui
                // resetto array valori inseriti
                currentCombinationInserted = new int[valuesToGuess.Length];
            }
            // altrimenti resetto contatore digits
            currentGuessedDigit = (currentGuessedDigit + 1) % valuesToGuess.Length;
        }
    }

    private void GetCombinationNumber(int value) {
        if(currentDigit == 0 && value < 0) {
            currentDigit = 40;
        }

        currentDigit = (currentDigit + value) % 40;
        // Debug.Log(currentDigit);
    }

    private bool CompareGuessedWithValues() {
        for(int i = 0; i < valuesToGuess.Length; i++) {
            if (valuesToGuess[i] != currentCombinationInserted[i]) {
                // if(debug) Debug.Log($"Compare {valuesToGuess[i]} w {currentCombinationInserted[i]}");
                return false;
            }
        }

        return true;
    }
}
