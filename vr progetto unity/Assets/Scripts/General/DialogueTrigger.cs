using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class delay
    {
        public float time;
        public int index = -1;
    }
    [System.Serializable]
    public class Dialogo
    {
        //public string nomeDialogo; // opzionale, per identificare il dialogo
        //public List<string> frasi;
        public List<AudioClip> frasi;
        public PuzzleInteraction[] puzzles;
        public  delay delay;
        public ItemType[] itemToAnnotateAfterDialog = new ItemType[0];
        public NotebookNotes.NotesProgress progressToUnlockAfterDialog = NotebookNotes.NotesProgress.None;
    }
    public List<Dialogo> dialogue;
    public AudioSource source;
    public Animator animator;
    public string nextScene = null;
    public void Awake()
    {
        source = this.gameObject.GetComponent<AudioSource>();
        if(animator == null)
            animator = this.gameObject.GetComponent<Animator>();
    }

    public void OnInteract(PlayerInteract playerInteract)
    {
        startDialogue();
    }

    public void startDialogue()
    {
        if(dialogue.Count > 0 && source != null)
        {
            //Time.timeScale = 0f;
            if(Player.Instance != null) {
                Player.Instance.SetState(Player.PlayerState.Dialog);
            }
            UiManager.Instance.startDialogue(dialogue, source, animator, nextScene);
            //if (index > dialogue.Count - 1){ index = dialogue.Count - 1; } //reset to last dialogue
        }
    }
}
