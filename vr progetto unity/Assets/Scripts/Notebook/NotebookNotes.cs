using UnityEngine;

public class NotebookNotes : MonoBehaviour {

    public enum NotesProgress {
        Ticket, 
        Woman,
        Barman,
        Hat,
        None
    }

    [SerializeField] private GameObject[] notesToShow;

    private void Awake() {
        foreach (var note in notesToShow) {
            note.SetActive(false);
        }
    }

    public void UnlockNewProgress(NotesProgress progressUnlocked) {
        NotebookManager.Instance.OpenNotebook(1);

        int storyProgressUnlocked = (int)progressUnlocked;
        notesToShow[storyProgressUnlocked].SetActive(true);

        // Avvia animazione testo
        TypewriterText typewriter = notesToShow[storyProgressUnlocked].GetComponentInChildren<TypewriterText>();
        if (!typewriter.active) return;

        if (typewriter != null) {
            typewriter.Play();
            NotebookManager.Instance.PlayLongWriteSound();

            float writingDuration = 3f; // per ora fissa
            NotebookManager.Instance.SetBusyForSeconds(writingDuration);
        }
    }
}
