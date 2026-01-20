using UnityEngine;

public class NotebookNotes : MonoBehaviour {
    public int storyProgress;

    [SerializeField] private GameObject[] notesToShow;

    private void Awake() {
        storyProgress = 0;

        foreach (var note in notesToShow) {
            note.SetActive(false);
        }
    }

    public void UnlockNewProgress() {
        // suono scrittura taccuino
        if (storyProgress >= notesToShow.Length) return;

        notesToShow[storyProgress].SetActive(true);

        // Avvia animazione testo
        TypewriterText typewriter = notesToShow[storyProgress].GetComponentInChildren<TypewriterText>();
        if (typewriter != null) {
            typewriter.Play();
            NotebookManager.Instance.PlayWriteSound();

            float writingDuration = 3f; // per ora fissa
            NotebookManager.Instance.SetBusyForSeconds(writingDuration);
        }

        storyProgress++;
    }
}
