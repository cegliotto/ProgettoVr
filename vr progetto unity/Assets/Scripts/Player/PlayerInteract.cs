using UnityEngine;

public class PlayerInteract : MonoBehaviour {

    [SerializeField] private float interactionDistance; // distanza del ray cast per check interazione
    [SerializeField] private Transform grabPoint; // Punto in cui portare oggetto grabbato
    private Ray interactionRay;

    [SerializeField] private GrabbableItem currentGrabbableItem;
    [SerializeField] private Transform itemFocus; // item attualmente puntato dal player

    [SerializeField] private LayerMask notInteractableMask;

    private void Update() {
        CheckInteraction();
    }

    public void Interact() { // Se ho premuto il tasto di interazione
        if(currentGrabbableItem == null) { // Se non ho nessun oggetto in mano
            if(itemFocus != null) { // controllo se mi sto focalizzando su un oggetto
                // Debug.Log($"interagendo con{itemFocus.name}");
                itemFocus.GetComponent<IInteractable>().OnInteract(this); // In caso positivo allora eseguo il suo metodo Interact
            }
        }
        else { // Ho un oggetto in mano
            TryGrab(currentGrabbableItem); // Provo a rilasciarlo
        }
    }

    private void CheckInteraction() {
        // si ricava posizione del centro dello schermo
        Vector2 centreScreenPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        // Si ottiene raggio che passa dal centro della camera al centro dello schermo ottenuto prima
        interactionRay = Camera.main.ScreenPointToRay(centreScreenPosition);

        int interactableMask = ~notInteractableMask; // Voglio che ci siano alcuni oggetti non interagibili
        // Che quindi il loro collider deve essere bypassato. Quindi il raycast interagisce con tutti i gameobject
        // tranne quelli che hanno il layer di tipo "notInteractableMask"

        if (Physics.Raycast(interactionRay, out RaycastHit hitInfo, interactionDistance, interactableMask)) {
            Debug.Log(hitInfo.collider.name);
            if (hitInfo.transform.TryGetComponent<IInteractable>(out IInteractable interactableItem)) {
                itemFocus = hitInfo.transform;
            }
            else {
                itemFocus = null;
            }
        }
        else {
            itemFocus = null;
        }
    }

    public void TryGrab(GrabbableItem obj) {
        // check su currentstate qui se non puo' grabbare in certe situzioni

        if (currentGrabbableItem != null) { // Se ho gia' qualcosa

            // controllo se posso rilasciare
            
            // rilascio
            obj.Release();
            currentGrabbableItem = null;

            // cambio FSM player?

            return;
        }

        // prendo oggetto e lo assegno alla variabile
        obj.Grab(grabPoint);
        currentGrabbableItem = obj;
        // cambio FSM player?
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(interactionRay);
    }
}
