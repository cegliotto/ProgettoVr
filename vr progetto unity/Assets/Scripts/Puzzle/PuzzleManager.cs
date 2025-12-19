using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [SerializeField] private string trainSceneName;
    private Player.PlayerInfo savedPlayerInfo; // Informazioni di posizione e orientamento del player per il cambio di scena

    private PuzzleType currentPuzzle;
    private void Awake() {

        if(Instance != null) { // Se c'e' gia' istanza 
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // per non distruggerlo al cambio di scena
    }

    public void StartPuzzle(string puzzleScene, PuzzleType puzzle) { // Richiamata in puzzleInteraction, dove passo il nome della scena
        savedPlayerInfo = Player.Instance.SaveInfo(); // Salvo nella variabile specificata le info di posizione e orientamento del player
        currentPuzzle = puzzle; // Mi segno il puzzle corrente

        // Per puzzleSafeMultiple
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(puzzleScene, LoadSceneMode.Single); // effettuo il load della scena
    }

    public void CompletePuzzle(/* ScriptableObject reference da passare */) {
        // Per puzzleSafeMultiple
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // inserimento in notebook di item tramite riferimento dello scriptableObject
        Debug.Log("Aggiornamento notebook");
        // carico scena treno
        SceneManager.sceneLoaded += OnTrainSceneLoaded; // Sottoscrivo evento -> in modo che quando la scena viene caricata
        // eseguo il metodo specificato
        SceneManager.LoadScene(trainSceneName, LoadSceneMode.Single);
    }

    private void OnTrainSceneLoaded(Scene arg0, LoadSceneMode arg1) {
        SceneManager.sceneLoaded -= OnTrainSceneLoaded; // tolgo sottoscrizione, in modo da evitrare problemi con altre scene
        // Aggiorno info del plyaer in modo che vada nella posizione prima del caricamento della scena
        Player.Instance.LoadInfo(savedPlayerInfo);

        // far partire animazione di oggetto specifico
        PuzzleInteraction[] puzzles = FindObjectsByType<PuzzleInteraction>(FindObjectsSortMode.None);

        foreach(PuzzleInteraction puzzle in puzzles) {
            if(puzzle.GetPuzzleType() == currentPuzzle) {
                // fai partire animazione
                puzzle.StartSolvedAnimation();
            }
        }
    }
}
