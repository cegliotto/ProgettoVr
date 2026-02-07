using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    //[SerializeField] private string trainSceneName;
    [SerializeField] private Camera mainCamera;
    private Camera puzzleCamera = null;
    private GameObject puzzleObj = null;
    private Player.PlayerInfo savedPlayerInfo; // Informazioni di posizione e orientamento del player per il cambio di scena


    private PuzzleType currentPuzzle;
    private List<PuzzleType> solvedPuzzles; // Lista che contiene SOLO i puzzle risolti
    private List<PuzzleType> unlockedPuzzles; // Lista che contiene SOLO i puzzle unlocked

    private void Awake() {
        if(Instance != null) { // Se c'e' gia' istanza 
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // per non distruggerlo al cambio di scena

        puzzleCamera = null;
        solvedPuzzles = new List<PuzzleType>();
        unlockedPuzzles = new List<PuzzleType>();
    }

    public void StartPuzzle(GameObject puzzleObjRef,Camera puzzleCameraRef, PuzzleType puzzle) { // Richiamata in puzzleInteraction, dove passo il nome della scena
        if(Player.Instance != null) {
            savedPlayerInfo = Player.Instance.SaveInfo(); // Salvo nella variabile specificata le info di posizione e orientamento del player
            Player.Instance.playerState = Player.PlayerState.Pause;
        }
        currentPuzzle = puzzle; // Mi segno il puzzle corrente

        // Per puzzleSafeMultiple
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // // caricamento della scena mediante levelLoader per fade-in / fade-out
        // if (LevelLoader.Instance != null)
        //     LevelLoader.Instance.LoadNextScene(puzzleCamera); // effettuo il load della scena
        // else
        //     SceneManager.LoadScene(puzzleCamera, LoadSceneMode.Single);
        puzzleObj = puzzleObjRef;
        puzzleObj.SetActive(true);

        puzzleCamera = puzzleCameraRef;
        mainCamera.enabled = false;
        puzzleCamera.enabled = true;
    }

    // A puzzle non completato, utente vuole solo tornare a scena del treno
    public void ExitFromPuzzle() {
        // Per puzzleSafeMultiple
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(Player.Instance != null) {
            Player.Instance.playerState = Player.PlayerState.Idle;
        }
        //SceneManager.sceneLoaded += OnTrainSceneLoadedExit;
        // if (LevelLoader.Instance != null)
        //     LevelLoader.Instance.LoadNextScene(trainSceneName);
        // else
        //     SceneManager.LoadScene(trainSceneName, LoadSceneMode.Single);
        mainCamera.enabled = true;
        puzzleCamera.enabled = false;
        if (puzzleCamera != null){ puzzleCamera = null; }

        puzzleObj.SetActive(false);
        puzzleObj = null;
    }

    // A puzzle completato
    public void CompletePuzzle() {
        // Per puzzleSafeMultiple
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // imposto il currentPuzzle come solved
        if (!solvedPuzzles.Contains(currentPuzzle)) {
            solvedPuzzles.Add(currentPuzzle);
        }
        else {
            Debug.LogWarning("NON DOVREI POTER COMPLETARE LO STESSO PUZZLE 2 VOLTE");
        }

        if(Player.Instance != null) {
            Player.Instance.playerState = Player.PlayerState.Idle;
        }
        // carico scena treno
        //SceneManager.sceneLoaded += OnTrainSceneLoadedCompleted; // Sottoscrivo evento -> in modo che quando la scena viene caricata
        // // caricamento della scena mediante levelLoader per fade-in / fade-out
        // if(LevelLoader.Instance != null)
        //     LevelLoader.Instance.LoadNextScene(trainSceneName);
        // else
        //     SceneManager.LoadScene(trainSceneName, LoadSceneMode.Single);
        mainCamera.enabled = true;
        puzzleCamera.enabled = false;
        if (puzzleCamera != null){ puzzleCamera = null; }

        puzzleObj.SetActive(false);
        puzzleObj = null;

        OnTrainSceneLoadedCompleted();
    }

    private void OnTrainSceneLoadedExit(Scene arg0, LoadSceneMode arg1) {
        SceneManager.sceneLoaded -= OnTrainSceneLoadedExit; // tolgo sottoscrizione, in modo da evitrare problemi con altre scene
        // Aggiorno info del plyaer in modo che vada nella posizione prima del caricamento della scena
        StartCoroutine(RestorePlayerRoutine());
    }

    private void OnTrainSceneLoadedCompleted() {
        //SceneManager.sceneLoaded -= OnTrainSceneLoadedCompleted; // tolgo sottoscrizione, in modo da evitrare problemi con altre scene

        // far partire animazione di oggetto specifico
        PuzzleInteraction[] puzzles = FindObjectsByType<PuzzleInteraction>(FindObjectsSortMode.None);

        foreach(PuzzleInteraction puzzle in puzzles) {
            if(puzzle.GetPuzzleType() == currentPuzzle) {
                // fai partire animazione
                puzzle.StartSolvedAnimation();
            }
        }

        // Aggiorno info del player in modo che vada nella posizione prima del caricamento della scena
        StartCoroutine(RestorePlayerRoutine());
    }

    private IEnumerator RestorePlayerRoutine() {
        // Al fine di evitare movimento della camera durante schermo nero
        // blocco inizialmente il player
        if (Player.Instance != null) {
            Player.Instance.playerState = Player.PlayerState.Pause;
            Player.Instance.LoadInfo(savedPlayerInfo);
        }

        // relativo a tempo impiegato da level loader
        float waitTime = 1.1f;
        yield return new WaitForSeconds(waitTime);

        // dopo che il caricamento e' finito riattivo il player
        if (Player.Instance != null) {
            // Ricarico nuovamente le informazioni per sicurezza
            Player.Instance.LoadInfo(savedPlayerInfo);
            // sblocco il player
            Player.Instance.playerState = Player.PlayerState.Idle;
        }
    }

    public bool isPuzzleSolved(PuzzleType puzzleToCheck) {
        return solvedPuzzles.Contains(puzzleToCheck);
    }

    public bool isPuzzleUnlocked(PuzzleType puzzleToCheck) {
        return unlockedPuzzles.Contains(puzzleToCheck);
    }

    public void AddUnlockedPuzzle(PuzzleType puzzleToSignAsUnlocked) {
        unlockedPuzzles.Add(puzzleToSignAsUnlocked);
    }

    public bool HasAnyPuzzleBeenSolved() {
        return solvedPuzzles.Count > 0;
    }
}
