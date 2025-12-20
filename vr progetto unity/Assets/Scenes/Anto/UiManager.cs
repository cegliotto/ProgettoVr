using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public float counter = 0f;
    void Update()
    {
        counter -= Time.deltaTime;
        if (currentDialogue != null)
        {
            if (counter <= 0f)
            {
                Debug.Log("pro");
                nextDialogue();
                counter = 2f;
            }
        }
    }

    #region dialoghi
    List<string> currentDialogue;
    int currentDialogueIndex = 0;
    public TextMeshProUGUI dialogueTextMesh = null;
    public void StartDialogue(List<string> dialogue)
    {
        counter = 2f;
        DialogueBox.gameObject.SetActive(true);

        //Time.timeScale = 0f;
        //InputManager.Instance.DisableAllInputs();
        EventSystem.current.SetSelectedGameObject(null);
        currentDialogue = dialogue;
        dialogueTextMesh.text = currentDialogue[0];
        //InputManager.Instance.DisablePause();

    }

    void nextDialogue()
    {
        if(currentDialogueIndex < currentDialogue.Count -1)
        {
            currentDialogueIndex++;
            dialogueTextMesh.text = currentDialogue[currentDialogueIndex];
        }
        else
        {
            Debug.Log("end");
            endDialogue();
        }

    }

    void endDialogue()
    {
        Time.timeScale = 1f;
        //InputManager.Instance.EnableAllInputs();
        DialogueBox.gameObject.SetActive(false);
        currentDialogue = null;
        currentDialogueIndex = 0;
        //InputManager.Instance.EnablePause();
    }
    #endregion
}
