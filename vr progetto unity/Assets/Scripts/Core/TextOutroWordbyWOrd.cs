using System.Collections;
using TMPro;
using UnityEngine;

public class WordByWordText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    [TextArea(5, 15)]
    public string fullText;

    public float wordDelay = 0.15f;
    public float cluePause = 2f;

    // Indizi (scritti ESATTAMENTE come nel testo)
    private string[] clues =
    {
        "biglietto ferroviario,",
        "bastone",
        "borsa,",
        "orologio,",
        "catena d'oro",
        "occhiali d'oro,",
        "cappello di castoro nero,"
    };

    [Header("Oggetti 3D")]
    public GameObject[] clueObjects; // Assicurati che sia nello stesso ordine di 'clues'

    void Start()
    {
        textComponent.text = "";
        StartCoroutine(ShowTextWordByWord());
    }

    IEnumerator ShowTextWordByWord()
    {
        string[] words = fullText.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            bool isClue = false;

            foreach (string clue in clues)
            {
                string[] clueWords = clue.Split(' ');

                if (i + clueWords.Length - 1 < words.Length)
                {
                    bool match = true;

                    for (int j = 0; j < clueWords.Length; j++)
                    {
                        if (words[i + j] != clueWords[j])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        isClue = true;

                        // Scrive l'indizio in grassetto e colorato
                        textComponent.text += "<b><color=#5A3E2B><size=90%>";

                        for (int j = 0; j < clueWords.Length; j++)
                        {
                            textComponent.text += clueWords[j] + " ";
                        }
                        textComponent.text += "</size></color></b>";

                        // Mostra l'oggetto 3D corrispondente
                        int clueIndex = System.Array.IndexOf(clues, clue);
                        if (clueIndex >= 0 && clueIndex < clueObjects.Length && clueObjects[clueIndex] != null)
                        {
                            clueObjects[clueIndex].SetActive(true);
                        }

                        // Pausa lunga sull'indizio
                        yield return new WaitForSeconds(cluePause);

                        i += clueWords.Length - 1;
                        break;
                    }
                }
            }

            if (!isClue)
            {
                textComponent.text += words[i] + " ";
                yield return new WaitForSeconds(wordDelay);
            }
        }
    }
}
