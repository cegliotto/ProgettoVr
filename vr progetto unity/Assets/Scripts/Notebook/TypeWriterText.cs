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
        fullText = AddSizeToNumbers(fullText);

        textComponent.text = "";
    }

    public void Play() {
        StopAllCoroutines();
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText() {
        textComponent.text = "";

        bool insideTag = false;

        foreach (char c in fullText) {

            textComponent.text += c;

            if (c == '<') {
                insideTag = true;
            }
            else if (c == '>') {
                insideTag = false;
            }

            if (!insideTag) {
                yield return new WaitForSeconds(letterDelay);
            }
        }
    }

    private string AddSizeToNumbers(string input) {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (char c in input) {
            if (char.IsDigit(c)) {
                sb.Append("<size=110%>");
                sb.Append(c);
                sb.Append("</size>");
            }
            else {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
