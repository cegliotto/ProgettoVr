using System.Collections;
using TMPro;
using UnityEngine;

public class CharacterByCharacterIntroText : MonoBehaviour
{
    [Header("UI Text")]
    public TextMeshProUGUI textComponent;
    [TextArea(5, 10)]
    public string fullText;
    public float charDelay = 0.05f;

    [Header("Button Settings")]
    public GameObject continueButton;
    public CanvasGroup buttonCanvasGroup;
    public float buttonDelay = 1f;
    public float fadeDuration = 0.5f;

    void Awake()
    {
        // Nascondi tutto all'inizio
        if (continueButton != null)
            continueButton.SetActive(false);

        if (buttonCanvasGroup != null)
            buttonCanvasGroup.alpha = 0f;
    }

    void Start()
    {
        if (textComponent == null)
        {
            Debug.LogError("Assegna il TextComponent nell'Inspector!");
            return;
        }

        textComponent.text = "";
        StartCoroutine(ShowTextCharacterByCharacter());
    }

    IEnumerator ShowTextCharacterByCharacter()
    {
        
        textComponent.text = fullText;
        textComponent.maxVisibleCharacters = 0;

        for (int i = 0; i <= fullText.Length; i++)
        {
            textComponent.maxVisibleCharacters = i;
            yield return new WaitForSeconds(charDelay);
        }

        // Attesa prima di mostrare il bottone
        yield return new WaitForSeconds(buttonDelay);

        if (continueButton != null && buttonCanvasGroup != null)
        {
            continueButton.SetActive(true);
            yield return StartCoroutine(FadeInButton());
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