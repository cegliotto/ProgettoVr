using System.Collections;
using UnityEngine;

public class PickUpItem : MonoBehaviour, IInteractable {

    [SerializeField] private bool debug;
    // Eventuale enum o scriptableObject associato
    [SerializeField] private ItemType itemType;

    [SerializeField] private bool isTicket;

    private void Awake() {
        if(NotebookManager.Instance != null) {
            if (NotebookManager.Instance.AlreadyPickedUp(this.itemType)) { // Se questo oggetto e' gia' stato raccolto
                // lo distruggo -> necessario per quando si cambia scena per la risoluzione dei puzzle
                Destroy(gameObject);
            }
        }
    }

    public void OnInteract(PlayerInteract playerInteract) {
        if (!isTicket) {
            if (debug) {
                Debug.Log($"Raccolto oggetto : {gameObject.name}");
            }
            // Aggiunta in Notebook
            if (NotebookManager.Instance != null) {
                NotebookManager.Instance.OpenNotebook(0); // apro la pagina 0 del notebook

                NotebookManager.Instance.notebookItemsManager.OnItemPickedUp(this.itemType);
            }
        }
        else {
            if(NotebookManager.Instance != null) {
                NotebookManager.Instance.OpenNotebook(1); // apro pagina 1
                NotebookManager.Instance.notebookNotesManager.UnlockNewProgress();
            }
        }

        // Suono di raccolta
        Destroy(gameObject); // Distruzione dell'oggetto nel mondo
    }

    private IEnumerator OpenNotebookNextFrame() {
        yield return null; 
        NotebookManager.Instance.OpenNotebook();
    }
}
