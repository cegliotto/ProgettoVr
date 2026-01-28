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

        if (isTicket) { // Il ticket non deve essere aggiunto nel taccuino
            HandleTicket();
            return;
        }

        if (!CanBePicked()) { // per cappello
            return;
        }

        PickItem();
    }

    private bool CanBePicked() {
        if (itemType == ItemType.Cappello) { // il cappello puo' essere raccolto solo come ultimo oggetto
            return NotebookManager.Instance != null &&
                   NotebookManager.Instance.notebookItemsManager.itemsPickedUp >= 5;
        }

        return true;
    }

    private void PickItem() {
        if (debug) {
            Debug.Log($"Raccolto oggetto : {gameObject.name}");
        }

        if (NotebookManager.Instance != null) {
            NotebookManager.Instance.OpenNotebook(0);
            NotebookManager.Instance.notebookItemsManager.OnItemPickedUp(itemType);
        }

        SpawnNPC();

        // Suono di raccolta
        Destroy(gameObject);
    }

    private void SpawnNPC() {
        if(itemType == ItemType.Cappello) {
            if (StoryNPCSpawn.Instance != null) {
                StoryNPCSpawn.Instance.SpawnNPC(itemType); // faccio spawnare NPC in zona relativa a oggetto
            }
            return; // tanto il cappello a prescindere posso prenderlo se itemsPickedUp >=5
        }

        if (NotebookManager.Instance.notebookItemsManager.itemsPickedUp >= 5) { // se ho preso tutti gli oggetti tranne il 
            // cappello
            if (StoryNPCSpawn.Instance != null) {
                StoryNPCSpawn.Instance.SpawnNPC(itemType); // faccio spawnare NPC in zona relativa a oggetto
            }
        }
    }

    private void HandleTicket() {
        if (NotebookManager.Instance != null) {
            NotebookManager.Instance.OpenNotebook(1);
            NotebookManager.Instance.notebookNotesManager.UnlockNewProgress(NotebookNotes.NotesProgress.Ticket);
        }

        // Suono di raccolta
        Destroy(gameObject);
    }

    private IEnumerator OpenNotebookNextFrame() {
        yield return null; 
        NotebookManager.Instance.OpenNotebook();
    }
}
