using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Dialogo = DialogueTrigger.Dialogo;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }
    
    [SerializeField] private GameObject DialogueBox;
    
    
    public bool isInPause = false;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        dialogueTextMesh = DialogueBox.GetComponentInChildren<TextMeshProUGUI>();
    }

    #region dialoghi
    Dialogo currentDialogue;
    int currentDialogueIndex = 0;
    public TextMeshProUGUI dialogueTextMesh = null;
    
    public void startDialogue(List<Dialogo> dialogue, AudioSource source, Animator animator, String nextScene = null)
    {   
        if(currentDialogue == null){
            StartCoroutine(nextDialogue(dialogue, source, animator, nextScene)); 
        }
    }

    public IEnumerator nextDialogue(List<Dialogo> dialogue, AudioSource source,  Animator animator, String nextScene = null)
    {
        DialogueBox.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); //check
        currentDialogue = dialogue[0];

        while(currentDialogueIndex < currentDialogue.frasi.Count)
        {
            source.clip = currentDialogue.frasi[currentDialogueIndex];
            
            Debug.Log("isPlaying");
            //animator.SetBool("isTalking", true);
            source.Play();
            yield return new WaitWhile(() => source.isPlaying);
            if (currentDialogueIndex == currentDialogue.delay.index && currentDialogue.delay.time >0f)
            {   Debug.Log("wait");
                yield return new WaitForSecondsRealtime(currentDialogue.delay.time);
                Debug.Log("end wait");
            }
            currentDialogueIndex++;
            
        }
        
        if(currentDialogueIndex >= currentDialogue.frasi.Count)
        {
            if (currentDialogue.puzzles != null){
                foreach (PuzzleInteraction puzzle in currentDialogue.puzzles) {
                    puzzle.unlocked = true; // Segno come sbloccato
                    // lo segno anche in puzzle manager per coerenza al cambio di scena
                    if(PuzzleManager.Instance != null) {
                        PuzzleManager.Instance.AddUnlockedPuzzle(puzzle.GetPuzzleType());
                    }
                }
            }

            if(NotebookManager.Instance != null) {
                // Se il dialogo deve far annotare un nuovo oggetto, si fa annotare
                if (currentDialogue.itemToAnnotateAfterDialog.Length > 0) {
                    foreach (ItemType itemToAnnotate in currentDialogue.itemToAnnotateAfterDialog) {
                        NotebookManager.Instance.notebookItemsManager.AnnotateNewItem(itemToAnnotate);
                    }
                }
                // Se il dialogo deve sbloccare nuova nota nel notebook, si sblocca quella associata al dialogo
                if (currentDialogue.progressToUnlockAfterDialog != NotebookNotes.NotesProgress.None) {
                    NotebookManager.Instance.notebookNotesManager.UnlockNewProgress(currentDialogue.progressToUnlockAfterDialog);
                }
            }

            endDialogue();
            if (dialogue.Count > 1){dialogue.RemoveAt(0);} //rimuove il dialogo solo se non è l'ultimo
            //animator.SetBool("isTalking", false);

            if (!string.IsNullOrEmpty(nextScene))
            {
                // caricamento della scena mediante levelLoader per fade-in / fade-out
                if (LevelLoader.Instance != null)
                    LevelLoader.Instance.LoadNextScene(nextScene); // effettuo il load della scena
                else
                    SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
            }
        }
        yield return null;
    }
    public void endDialogue()
    {
        // Time.timeScale = 1f;
        if (Player.Instance != null) {
            if(Player.Instance.playerState == Player.PlayerState.Dialog) {
                Player.Instance.SetState(Player.PlayerState.Idle);
            }
        }
        DialogueBox.gameObject.SetActive(false);
        currentDialogue = null;
        currentDialogueIndex = 0;
        //InputManager.Instance.EnablePause();
        //InputManager.Instance.EnableAllInputs();
    }
    #endregion
}
