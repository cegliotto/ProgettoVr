using UnityEngine;

public class OpeningOnStart : MonoBehaviour
{
    [SerializeField] private float time = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Invoke("OpenNotebookOnStart", time);
    }

    private void OpenNotebookOnStart() {
        if (NotebookManager.Instance == null) return;

        NotebookManager.Instance.OpenNotebook();
    }
}
