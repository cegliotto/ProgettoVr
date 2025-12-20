using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public List<string> dialogue;
    void OnTriggerEnter(Collider other)
    {
        if(dialogue.Count > 0)
        {
            UiManager.Instance.StartDialogue(dialogue);
        }
    }
}
