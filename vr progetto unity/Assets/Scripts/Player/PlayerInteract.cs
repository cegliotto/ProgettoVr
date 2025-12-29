using System;
using System.Collections.Generic;
using NUnit.Framework;
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

    GameObject lastObj;
    Queue<string> oldLayer = new Queue<string>();
    bool hasHit = false;

    private void CheckInteraction() {
        // si ricava posizione del centro dello schermo
        Vector2 centreScreenPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        // Si ottiene raggio che passa dal centro della camera al centro dello schermo ottenuto prima
        interactionRay = Camera.main.ScreenPointToRay(centreScreenPosition);
        int interactableMask = ~notInteractableMask; // Voglio che ci siano alcuni oggetti non interagibili
        // Che quindi il loro collider deve essere bypassato. Quindi il raycast interagisce con tutti i gameobject
        // tranne quelli che hanno il layer di tipo "notInteractableMask"

        if (Physics.Raycast(interactionRay, out RaycastHit hitInfo, interactionDistance, interactableMask)) {
            
            if (!hasHit)
            {   //la prima volta che becca un oggetto cambia il layer in modo da mostrare l'outline
                hasHit = true;
                
                lastObj = hitInfo.collider.gameObject;
                
                //setta al game object e tutti i suoi figli il layer outline e memorizza i layer precedenti
                oldLayer.Enqueue(LayerMask.LayerToName(hitInfo.collider.gameObject.layer));
                hitInfo.collider.gameObject.layer = LayerMask.NameToLayer("OutLine");
                if (hitInfo.transform.childCount > 0)
                {
                    foreach (Transform child in hitInfo.transform){
                        oldLayer.Enqueue(LayerMask.LayerToName(child.gameObject.layer));
                        child.gameObject.layer = LayerMask.NameToLayer("OutLine");
                    }                    
                }
            }


            if (hitInfo.transform.TryGetComponent<IInteractable>(out IInteractable interactableItem)) {
                itemFocus = hitInfo.transform;
            }
            else {
                itemFocus = null;
            }
        }
        else {
            itemFocus = null;
            //probabilmente has hit non serve ma il codice è più leggibile
            hasHit = false;
            if (lastObj != null && oldLayer.Count > 0)
            {
                //restituisce all'oggetto e tutti i figli i layer precedenti
                lastObj.layer = LayerMask.NameToLayer(oldLayer.Dequeue());
                if (lastObj.transform.childCount > 0)
                {
                    foreach (Transform child in lastObj.transform){
                        child.gameObject.layer = LayerMask.NameToLayer(oldLayer.Dequeue());
                    }                    
                }
                lastObj = null;
                oldLayer.Clear();
            }
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
