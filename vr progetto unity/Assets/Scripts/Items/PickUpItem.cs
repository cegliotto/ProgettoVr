using UnityEngine;

public class PickUpItem : MonoBehaviour, IInteractable {

    [SerializeField] private bool debug;
    // Eventuale enum o scriptableObject associato

    public void OnInteract(PlayerInteract playerInteract) {
        if (debug) {
            Debug.Log($"Raccolto oggetto : {gameObject.name}");
        }
        // Aggiunta in Notebook dello scriptableObject/enum/qualsiasi cosa 

        // Suono di raccolta

        Destroy(gameObject); // Distruzione dell'oggetto nel mondo
    }
}
