using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    public List<string> dialogue;

    PlayerInteract playerInteract;
    public void OnInteract(PlayerInteract playerInteract)
    {
        if(dialogue.Count > 0)
        {
            UiManager.Instance.nextDialogue(dialogue);
        }
        
    }
}
