using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Dialogo = DialogueTrigger.Dialogo;

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
    
    public void startDialogue(List<Dialogo> dialogue, AudioSource source)
    {   
        if(currentDialogue == null){
            StartCoroutine(nextDialogue(dialogue, source)); 
        }
    }

    public IEnumerator nextDialogue(List<Dialogo> dialogue, AudioSource source)
    {
        DialogueBox.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); //check
        currentDialogue = dialogue[0];

        while(currentDialogueIndex < currentDialogue.frasi.Count)
        {
            source.clip = currentDialogue.frasi[currentDialogueIndex];
            
            Debug.Log("isPlaying");
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
            if (currentDialogue.puzzle != null){ currentDialogue.puzzle.enabled = true; }
            endDialogue();
            if (dialogue.Count > 1){dialogue.RemoveAt(0);} //rimuove il dialogo solo se non è l'ultimo
        }
        yield return null;
    }
    public void endDialogue()
    {
        Time.timeScale = 1f;
        DialogueBox.gameObject.SetActive(false);
        currentDialogue = null;
        currentDialogueIndex = 0;
        //InputManager.Instance.EnablePause();
        //InputManager.Instance.EnableAllInputs();
    }
    #endregion
}
