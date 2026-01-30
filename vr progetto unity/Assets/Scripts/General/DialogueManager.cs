using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public int numDialogues;
    public static List<int> toRemove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (toRemove == null)
            toRemove = new List<int>();

        for (int i = 0; i < numDialogues; i++)
        {
            toRemove.Add(0); //memorizza per ciascun dialogo il numero di dialoghi già eseguiti
        }
    }


}
