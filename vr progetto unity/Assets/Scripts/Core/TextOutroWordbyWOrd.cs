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
    
    [Header("Hover Animation")]
    public float hoverAmplitude = 0.1f; 
    public float hoverSpeed = 2f;

    [Header("Button Settings")]
    public GameObject continueButton;
    public CanvasGroup buttonCanvasGroup;
    public float buttonDelay = 1f;
    public float fadeDuration = 0.5f;

    void Awake()
    {
        // si nasconde tutto all'inizio
        if (continueButton != null)
            continueButton.SetActive(false);

        if (buttonCanvasGroup != null)
            buttonCanvasGroup.alpha = 0f;
    }

    void Start()
    {
        textComponent.text = "";
        textComponent.maxVisibleCharacters = 0;

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
                    // Pulse e oscillazione
                    StartCoroutine(AnimateObjectSequence(clueObjects[matchedClueIndex]));
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
        // Attesa prima di mostrare il bottone
        yield return new WaitForSeconds(buttonDelay);

        if (continueButton != null && buttonCanvasGroup != null)
        {
            continueButton.SetActive(true);
            yield return StartCoroutine(FadeInButton());
        }
    }

    //Pulse poi Hover infinito
    IEnumerator AnimateObjectSequence(GameObject obj)
    {
        obj.SetActive(true);

        // Pulse
        yield return StartCoroutine(PulseRoutine(obj));

        // si avvia l'oscillazione passando la posizione ATTUALE dell'oggetto per evitare salti improvvisi
        StartCoroutine(HoverRoutine(obj));
    }

    IEnumerator PulseRoutine(GameObject obj)
    {
        Vector3 originalScale = obj.transform.localScale;
        float elapsed = 0;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curve = Mathf.Sin(t * Mathf.PI);
            obj.transform.localScale = Vector3.Lerp(originalScale, Vector3.Scale(originalScale, pulseScale), curve);
            yield return null;
        }
        obj.transform.localScale = originalScale;
    }

    IEnumerator HoverRoutine(GameObject obj)
    {
        Vector3 startPos = obj.transform.position;
        float randomOffset = Random.Range(0f, 10f);

        // Variabile per far entrare l'oscillazione gradualmente 
        float motionIntensity = 0f;
        float fadeInSpeed = 0.5f; //In 2 secondi l'oscillazione arriva a regime

        while (obj != null && obj.activeInHierarchy)
        {
            //imcremento l'intensità per evitare lo scatto iniziale
            motionIntensity = Mathf.MoveTowards(motionIntensity, 1f, Time.deltaTime * fadeInSpeed);

            float time = (Time.time + randomOffset) * hoverSpeed;

            //Calcolo l'offset (lo scostamento)
            float offsetX = Mathf.Cos(time * 0.8f) * (hoverAmplitude * 0.5f);
            float offsetY = Mathf.Sin(time) * hoverAmplitude;
            //offset moltiplicato per intensità corrente
            obj.transform.position = startPos + new Vector3(offsetX, offsetY, 0) * motionIntensity;

            yield return null;
        }
    }

    IEnumerator FadeInButton()
    {
        float elapsed = 0f;
        buttonCanvasGroup.interactable = false;
        buttonCanvasGroup.blocksRaycasts = false;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            buttonCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        buttonCanvasGroup.alpha = 1f;
        buttonCanvasGroup.interactable = true;
        buttonCanvasGroup.blocksRaycasts = true;
    }
}