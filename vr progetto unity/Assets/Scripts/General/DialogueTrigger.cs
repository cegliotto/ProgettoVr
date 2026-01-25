using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class Dialogo
    {
        //public string nomeDialogo; // opzionale, per identificare il dialogo
        public List<string> frasi;
        public PuzzleInteraction puzzle;
    }
    public List<Dialogo> dialogue;
    private int index = 0;

    PlayerInteract playerInteract;
    public void OnInteract(PlayerInteract playerInteract)
    {
        if(dialogue.Count > 0)
        {
            Time.timeScale = 0f;
            index = UiManager.Instance.nextDialogue(dialogue[index].frasi, index);
            if (index > dialogue.Count - 1){ index = dialogue.Count - 1; } //reset to last dialogue
        }
        
    }
}
