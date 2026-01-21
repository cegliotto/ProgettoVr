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

    public bool IsGrabbing() => currentGrabbableItem != null;

    private void Update() {
        CheckInteraction();
    }

    public void Interact() { // Se ho premuto il tasto di interazione
        if (Player.Instance.playerState == Player.PlayerState.Dialog
            || Player.Instance.playerState == Player.PlayerState.Pause) {
            return;
        }

        if (currentGrabbableItem == null) { // Se non ho nessun oggetto in mano
            if (itemFocus != null) { // controllo se mi sto focalizzando su un oggetto
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
    [SerializeField] Material outlineMat;

    private void CheckInteraction() {
        // si ricava posizione del centro dello schermo
        Vector2 centreScreenPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        // Si ottiene raggio che passa dal centro della camera al centro dello schermo ottenuto prima
        interactionRay = Camera.main.ScreenPointToRay(centreScreenPosition);

        if (Physics.Raycast(interactionRay, out RaycastHit hitInfo, interactionDistance)) {

            if (hitInfo.collider.gameObject != lastObj) { outlineCleanup(); }
            //if (!hasHit &&  hitInfo.collider.gameObject ha interactable)
            if (!hasHit) {   //la prima volta che becca un oggetto cambia il layer in modo da mostrare l'outline
                hasHit = true;

                lastObj = hitInfo.collider.gameObject;

                MeshFilter mf = lastObj.GetComponent<MeshFilter>();
                if (mf != null) {
                    // Vector3 center = mf.mesh.bounds.center; // object space
                    // outlineMat.SetVector("pivot", center);
                    Vector3 worldCenter = CalculateCombinedCenter(lastObj);
                    outlineMat.SetVector("pivot", worldCenter);
                }

                if (lastObj.GetComponent<GrabbableItem>() != null ||
                    lastObj.GetComponent<PickUpItem>() != null ||
                    lastObj.GetComponent<DialogueTrigger>() != null ||
                    lastObj.GetComponent<PuzzleInteraction>() != null) {
                    //setta al game object e tutti i suoi figli il layer outline e memorizza i layer precedenti
                    oldLayer.Enqueue(LayerMask.LayerToName(hitInfo.collider.gameObject.layer));
                    hitInfo.collider.gameObject.layer = LayerMask.NameToLayer("OutLine");
                    if (hitInfo.transform.childCount > 0) {
                        foreach (Transform child in hitInfo.transform) {
                            oldLayer.Enqueue(LayerMask.LayerToName(child.gameObject.layer));
                            child.gameObject.layer = LayerMask.NameToLayer("OutLine");
                        }
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
            outlineCleanup();
        }
    }

    public void outlineCleanup() {
        //probabilmente has hit non serve ma il codice è più leggibile
        hasHit = false;
        if (lastObj != null && oldLayer.Count > 0) {
            //restituisce all'oggetto e tutti i figli i layer precedenti
            lastObj.layer = LayerMask.NameToLayer(oldLayer.Dequeue());
            if (lastObj.transform.childCount > 0) {
                foreach (Transform child in lastObj.transform) {
                    child.gameObject.layer = LayerMask.NameToLayer(oldLayer.Dequeue());
                }
            }
            //rimuove riferimento all'oggetto e 
            lastObj = null;
            oldLayer.Clear();
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

    public void ForcedRelease() {
        currentGrabbableItem = null;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(interactionRay);
    }
    Vector3 CalculateCombinedCenter(GameObject root) {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return root.transform.position;

        Bounds combinedBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++) {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        return combinedBounds.center;
    }
}
