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

        // caricamento della scena mediante levelLoader per fade-in / fade-out
        if (LevelLoader.Instance != null)
            LevelLoader.Instance.LoadNextScene(puzzleScene); // effettuo il load della scena
        else
            SceneManager.LoadScene(puzzleScene, LoadSceneMode.Single);
    }

    // A puzzle non completato, utente vuole solo tornare a scena del treno
    public void ExitFromPuzzle() {
        // Per puzzleSafeMultiple
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.sceneLoaded += OnTrainSceneLoadedExit;

        if (LevelLoader.Instance != null)
            LevelLoader.Instance.LoadNextScene(trainSceneName);
        else
            SceneManager.LoadScene(trainSceneName, LoadSceneMode.Single);
    }

    // A puzzle completato
    public void CompletePuzzle() {
        // Per puzzleSafeMultiple
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // carico scena treno
        SceneManager.sceneLoaded += OnTrainSceneLoadedCompleted; // Sottoscrivo evento -> in modo che quando la scena viene caricata
        
        // caricamento della scena mediante levelLoader per fade-in / fade-out
        if(LevelLoader.Instance != null)
            LevelLoader.Instance.LoadNextScene(trainSceneName);
        else
            SceneManager.LoadScene(trainSceneName, LoadSceneMode.Single);
    }

    private void OnTrainSceneLoadedExit(Scene arg0, LoadSceneMode arg1) {
        SceneManager.sceneLoaded -= OnTrainSceneLoadedExit; // tolgo sottoscrizione, in modo da evitrare problemi con altre scene
        // Aggiorno info del plyaer in modo che vada nella posizione prima del caricamento della scena
        Player.Instance.LoadInfo(savedPlayerInfo);

        PuzzleInteraction[] puzzles = FindObjectsByType<PuzzleInteraction>(FindObjectsSortMode.None); 

        foreach (PuzzleInteraction puzzle in puzzles)
        {
            if (puzzle.GetPuzzleType() == currentPuzzle)
            {
                // Esegue l'animazione standard 
                puzzle.StartSolvedAnimation(); //

                // Sblocca la fisica solo se il componente esiste:  se non c'è PhysicalUnlock, non fa nulla
                if (puzzle.TryGetComponent<PhysicalUnlock>(out PhysicalUnlock unlocker))
                {
                    unlocker.Unlock(); //
                }
            }
        }
    }

    private void OnTrainSceneLoadedCompleted(Scene arg0, LoadSceneMode arg1) {
        SceneManager.sceneLoaded -= OnTrainSceneLoadedCompleted; // tolgo sottoscrizione, in modo da evitrare problemi con altre scene
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
