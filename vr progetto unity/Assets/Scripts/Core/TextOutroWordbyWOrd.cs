using System.Collections;
using TMPro;
using UnityEngine;

public class WordByWordText : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textComponent;

    [TextArea(5, 15)]
    public string fullText;

    [Header("Timing")]
    public float charDelay = 0.05f;
    public float cluePause = 2f;

    [Header("Clues")]
    public string[] clues = {
        "biglietto ferroviario,", "bastone", "borsa,", "orologio,",
        "catena d'oro", "occhiali d'oro,", "cappello di castoro nero,"
    };

    [Header("3D Objects")]
    public GameObject[] clueObjects;

    [Header("Animation Settings")]
    public float animationDuration = 0.8f;
    public Vector3 pulseScale = new Vector3(1.2f, 1.2f, 1.2f);

    void Start()
    {
        textComponent.text = "";
        textComponent.maxVisibleCharacters = 0;

        // oggetti invisibili all'inizio
        foreach (var obj in clueObjects) if (obj != null) obj.SetActive(false);

        StartCoroutine(ShowTextRoutine());
    }

    IEnumerator ShowTextRoutine()
    {
        string processedText = fullText;
        foreach (string clue in clues)
        {
            string formatted = $"<b><color=#A0522D><size=90%>{clue}</size></color></b>";
            processedText = processedText.Replace(clue, formatted);
        }

        textComponent.text = processedText;
        textComponent.ForceMeshUpdate();

        int totalCharacters = textComponent.textInfo.characterCount;
        int currentChar = 0;
        string plainText = textComponent.textInfo.textComponent.GetParsedText();

        while (currentChar < totalCharacters)
        {
            int matchedClueIndex = -1;
            int clueCharLength = 0;

            for (int i = 0; i < clues.Length; i++)
            {
                if (plainText.IndexOf(clues[i], currentChar) == currentChar)
                {
                    matchedClueIndex = i;
                    clueCharLength = clues[i].Length;
                    break;
                }
            }

            if (matchedClueIndex != -1)
            {
                for (int j = 0; j < clueCharLength; j++)
                {
                    currentChar++;
                    textComponent.maxVisibleCharacters = currentChar;
                    yield return new WaitForSeconds(charDelay);
                }

                if (matchedClueIndex < clueObjects.Length && clueObjects[matchedClueIndex] != null)
                {
                    // ANIMAZIONE PULSE
                    StartCoroutine(AnimateObject(clueObjects[matchedClueIndex]));
                }

                yield return new WaitForSeconds(cluePause);
            }
            else
            {
                currentChar++;
                textComponent.maxVisibleCharacters = currentChar;
                yield return new WaitForSeconds(charDelay);
            }
        }
    }

    IEnumerator AnimateObject(GameObject obj)
    {
        obj.SetActive(true);
        Vector3 originalScale = obj.transform.localScale;
        Renderer renderer = obj.GetComponentInChildren<Renderer>();

        float elapsed = 0;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            // Animazione Pulse 
            float curve = Mathf.Sin(t * Mathf.PI);
            obj.transform.localScale = Vector3.Lerp(originalScale, Vector3.Scale(originalScale, pulseScale), curve);

            yield return null;
        }

        obj.transform.localScale = originalScale;
    }
}