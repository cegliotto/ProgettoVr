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
    
    public void startDialogue(int dialogueId, List<Dialogo> dialogue, AudioSource source, Animator animator, String nextScene = null)
    {   
        if(currentDialogue == null){
            StartCoroutine(nextDialogue(dialogueId, dialogue, source, animator, nextScene)); 
        }
    }

    public IEnumerator nextDialogue(int dialogueId, List<Dialogo> dialogue, AudioSource source,  Animator animator, String nextScene = null)
    {
        MusicManager.Instance.DuckForDialogue(0.3f);
        DialogueBox.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); //check
        currentDialogue = dialogue[0];

        while(currentDialogueIndex < currentDialogue.frasi.Count)
        {
            source.clip = currentDialogue.frasi[currentDialogueIndex].clip;
            dialogueTextMesh.text = currentDialogue.frasi[currentDialogueIndex].text;
            
            Debug.Log("isPlaying");
            SetTalkingAnimation(dialogueId, true, animator);
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
                    if (puzzle.glow != null){ puzzle.glow.enabled = true; }
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
            if (dialogue.Count > 1){
                dialogue.RemoveAt(0);

                if(DialogueManager.toRemove.Count - 1  >= dialogueId)
                    DialogueManager.toRemove[dialogueId]++;
            } //rimuove il dialogo solo se non è l'ultimo
            SetTalkingAnimation(dialogueId, false, animator);

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
        MusicManager.Instance.UnduckAfterDialogue(0.4f);
        //InputManager.Instance.EnablePause();
        //InputManager.Instance.EnableAllInputs();
    }

    private void SetTalkingAnimation(int dialogueId, bool value, Animator anim) {
        Pendolare pendolare = anim.GetComponent<Pendolare>();
        bool isPendolareSitted = pendolare != null && pendolare.sitted;

        if (value && dialogueId != 3 && !isPendolareSitted) {
            Vector3 dir = Player.Instance.transform.position - anim.transform.position;
            dir.y = 0f;

            if (dir != Vector3.zero)
                anim.transform.rotation = Quaternion.LookRotation(dir);
        }

        if (anim.GetComponent<BarMan>() == null)
            anim.SetBool("isTalking", value);
        else
            anim.GetComponent<BarMan>().SetTalking(value);
    }
    #endregion
}
