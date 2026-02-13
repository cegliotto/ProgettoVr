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
        if (Player.Instance.playerState != Player.PlayerState.Pause)
        {
            CheckInteraction();
        }else
        {
            outlineCleanup();
        }
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
    private Dictionary<Renderer, uint> originalRenderingLayers = new();
    [SerializeField] private RenderingLayerMask outlineLayer;
    bool hasHit = false;
    //[SerializeField] Material outlineMat;

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

                if ((lastObj.TryGetComponent<GrabbableItem>(out var g) && g.isActiveAndEnabled) ||
                    (lastObj.TryGetComponent<PickUpItem>(out var p) && p.isActiveAndEnabled) ||
                    (lastObj.TryGetComponent<DialogueTrigger>(out var d) && d.isActiveAndEnabled) ||
                    (lastObj.TryGetComponent<PuzzleInteraction>(out var pi) && pi.isActiveAndEnabled && pi.unlocked)
                    ) {
                    //setta al game object e tutti i suoi figli il layer outline e memorizza i layer precedenti
                    Renderer[] renderers = lastObj.GetComponentsInChildren<Renderer>();

                    foreach (Renderer rend in renderers) {
                        if (!originalRenderingLayers.ContainsKey(rend)) {
                            originalRenderingLayers[rend] = rend.renderingLayerMask;
                            rend.renderingLayerMask |= outlineLayer.value;
                        }
                    }
                }
            }


            if (hitInfo.transform.TryGetComponent<PuzzleInteraction>(out var puzzleInteraction)) {
                itemFocus = (puzzleInteraction.enabled && puzzleInteraction.unlocked) ? hitInfo.transform : null;
            }
            else if (hitInfo.transform.TryGetComponent<IInteractable>(out IInteractable interactableItem)) {
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
        hasHit = false;

        foreach (var kvp in originalRenderingLayers) {
            if (kvp.Key != null)
                kvp.Key.renderingLayerMask = kvp.Value;
        }

        originalRenderingLayers.Clear();
        lastObj = null;
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
