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

    public int nextDialogue(Dialogo dialogue , int globalDialogueIndex)
    {
        DialogueBox.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        currentDialogue = dialogue;

        //dialogueTextMesh.text = currentDialogue[0];

        if(currentDialogueIndex < currentDialogue.frasi.Count)
        {
            dialogueTextMesh.text = currentDialogue.frasi[currentDialogueIndex];
            currentDialogueIndex++;
        }
        else
        {
            if (dialogue.puzzle != null){ dialogue.puzzle.enabled = true; }
            endDialogue();
            globalDialogueIndex++;
        }
        return globalDialogueIndex;
    }

    void endDialogue()
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
