using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    //[SerializeField] private string trainSceneName;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AudioListener mainListener;
    private Camera puzzleCamera = null;
    private AudioListener puzzleListener = null;
    GameObject pointer = null;
    GameObject fadeIn = null;
    private GameObject puzzleObj = null;
    private Player.PlayerInfo savedPlayerInfo; // position and orientation of the player

    private PuzzleType currentPuzzle;
    private List<PuzzleType> solvedPuzzles; // ONLY solved puzzle 
    private List<PuzzleType> unlockedPuzzles; // ONLY unlocked puzzle

    public bool isPlayingPuzzle;

    private void Awake() {
        if(Instance != null) { 
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // only one, don't destroy

        puzzleCamera = null;
        solvedPuzzles = new List<PuzzleType>();
        unlockedPuzzles = new List<PuzzleType>();
    }

    public void StartPuzzle(GameObject puzzleObjRef, Camera puzzleCameraRef, AudioListener puzzleListenerRef, PuzzleType puzzle) { 
        if(Player.Instance != null) {
            savedPlayerInfo = Player.Instance.SaveInfo(); // Position and orientation saved
            Player.Instance.playerState = Player.PlayerState.Pause;
        }
        currentPuzzle = puzzle;

        if(CursorManager.Instance != null) {
            CursorManager.Instance.SetContext(CursorContext.UI);
        }

        // upload scene with levelLoader for fade-in / fade-out
        puzzleObj = puzzleObjRef;
        puzzleObj.SetActive(true);

        fadeIn = LevelLoader.Instance.gameObject.transform.Find("CrossFade").gameObject;
        if (fadeIn != null){ fadeIn.SetActive(false);}
        
        pointer = UiManager.Instance.gameObject.transform.Find("Canvas/pointer").gameObject;
        if (pointer != null){ pointer.SetActive(false); }
        
        puzzleCamera = puzzleCameraRef;
        puzzleListener = puzzleListenerRef;
        mainCamera.enabled = false;
        puzzleCamera.enabled = true;
        mainListener.enabled = false;
        puzzleListener.enabled = true;

        isPlayingPuzzle = true; // you can't open the notebook during puzzle
    }

    // exit with puzzle unsolved
    public void ExitFromPuzzle() {
        // For puzzleSafeMultiple
        if (CursorManager.Instance != null) {
            CursorManager.Instance.SetContext(CursorContext.Gameplay);
        }

        if (Player.Instance != null) {
            Player.Instance.playerState = Player.PlayerState.Idle;
        }
        if (Pendolare.Instance.gotTicket && Pendolare.Instance != null)
        {
            Pendolare.Instance.MoveAndSit();
        }
        //SceneManager.sceneLoaded += OnTrainSceneLoadedExit;
        // if (LevelLoader.Instance != null)
        // LevelLoader.Instance.LoadNextScene(trainSceneName);
        // else
        // SceneManager.LoadScene(trainSceneName, LoadSceneMode.Single);

        if (fadeIn != null){ fadeIn.SetActive(true); }
        if (pointer != null){ pointer.SetActive(true); }

        mainCamera.enabled = true;
        puzzleCamera.enabled = false;
        mainListener.enabled = true;
        puzzleListener.enabled = false;
        if (puzzleCamera != null){ puzzleCamera = null; }

        puzzleObj.SetActive(false);
        puzzleObj = null;

        isPlayingPuzzle = false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene changed");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mainListener = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>();
    }

    // when puzzle is completed
    public void CompletePuzzle() {
        // For puzzleSafeMultiple
        if (CursorManager.Instance != null) {
            CursorManager.Instance.SetContext(CursorContext.Gameplay);
        }

        // set the currentPuzzle as solved
        if (!solvedPuzzles.Contains(currentPuzzle)) {
            solvedPuzzles.Add(currentPuzzle);
        }
        else {
            Debug.LogWarning("I SHOULD NOT BE ABLE TO COMPLETE THE SAME PUZZLE TWICE");
        }

        if(Player.Instance != null) {
            Player.Instance.playerState = Player.PlayerState.Idle;
        }
        
        if (Pendolare.Instance.gotTicket && Pendolare.Instance != null)
        {
            Pendolare.Instance.MoveAndSit(); 
        }
        
        //SceneManager.sceneLoaded += OnTrainSceneLoadedCompleted; // Subscribe to event -> so that when the scene is loaded
        // // loading the scene via levelLoader for fade-in / fade-out
        // if(LevelLoader.Instance != null)
        // LevelLoader.Instance.LoadNextScene(trainSceneName);
        // else
        // SceneManager.LoadScene(trainSceneName, LoadSceneMode.Single);
        if (fadeIn != null){ fadeIn.SetActive(true); }
        if (pointer != null){ pointer.SetActive(true); }
        
        mainCamera.enabled = true;
        puzzleCamera.enabled = false;
        mainListener.enabled = true;
        puzzleListener.enabled = false;

        if (puzzleCamera != null){ puzzleCamera = null; }

        puzzleObj.SetActive(false);
        puzzleObj = null;

        isPlayingPuzzle = false;

        OnTrainSceneLoadedCompleted();
    }

    private void OnTrainSceneLoadedExit(Scene arg0, LoadSceneMode arg1) {
        SceneManager.sceneLoaded -= OnTrainSceneLoadedExit; 
        
        StartCoroutine(RestorePlayerRoutine()); // coroutine to restore player position and orientation
    }

    private void OnTrainSceneLoadedCompleted() {
        //SceneManager.sceneLoaded -= OnTrainSceneLoadedCompleted; 

        // start animation of objects
        PuzzleInteraction[] puzzles = FindObjectsByType<PuzzleInteraction>(FindObjectsSortMode.None);

        foreach(PuzzleInteraction puzzle in puzzles) {
            if(puzzle.GetPuzzleType() == currentPuzzle) {
                // start animation
                puzzle.StartSolvedAnimation();
            }
        }

        // update player info
        StartCoroutine(RestorePlayerRoutine());
    }

    private IEnumerator RestorePlayerRoutine() {
        
        if (Player.Instance != null) {
            Player.Instance.playerState = Player.PlayerState.Pause;
            Player.Instance.LoadInfo(savedPlayerInfo);
        }

        // level loader time
        float waitTime = 1.1f;
        yield return new WaitForSeconds(waitTime);

        // player reactivation
        if (Player.Instance != null) {
           
            Player.Instance.LoadInfo(savedPlayerInfo);
           
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