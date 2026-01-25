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
        public PuzzleInteraction puzzle;
        public  delay delay;
    }
    public List<Dialogo> dialogue;
    public AudioSource source;
    public void Awake()
    {
        source = this.gameObject.GetComponent<AudioSource>();
    }
    public void OnInteract(PlayerInteract playerInteract)
    {
        if(dialogue.Count > 0 && source != null)
        {
            Time.timeScale = 0f;
            UiManager.Instance.startDialogue(dialogue, source);
            //if (index > dialogue.Count - 1){ index = dialogue.Count - 1; } //reset to last dialogue
        }
        
    }
}
