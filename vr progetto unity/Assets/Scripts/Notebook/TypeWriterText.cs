using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterText : MonoBehaviour {

    [SerializeField] private float letterDelay = 0.01f;

    private TMP_Text textComponent;
    private string fullText;

    void Awake() {
        textComponent = GetComponent<TMP_Text>();
        fullText = textComponent.text;
        textComponent.text = "";
    }

    public void Play() {
        StopAllCoroutines();
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText() {
        textComponent.text = "";

        foreach (char c in fullText) {
            textComponent.text += c;
            yield return new WaitForSeconds(letterDelay);
        }
    }
}
