using UnityEngine;

public class PickUpItem : MonoBehaviour, IInteractable {

    [SerializeField] private bool debug;
    // Eventuale enum o scriptableObject associato
    [SerializeField] private ItemType itemType;

    private void Awake() {
        if(NotebookManager.Instance != null) {
            if (NotebookManager.Instance.AlreadyPickedUp(this.itemType)) { // Se questo oggetto e' gia' stato raccolto
                // lo distruggo -> necessario per quando si cambia scena per la risoluzione dei puzzle
                Destroy(gameObject);
            }
        }
    }

    public void OnInteract(PlayerInteract playerInteract) {
        if (debug) {
            Debug.Log($"Raccolto oggetto : {gameObject.name}");
        }
        // Aggiunta in Notebook
        if(NotebookManager.Instance != null)
            NotebookManager.Instance.notebookItemsManager.OnItemPickedUp(this.itemType);

        // Suono di raccolta

        Destroy(gameObject); // Distruzione dell'oggetto nel mondo
    }
}
