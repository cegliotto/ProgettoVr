using UnityEngine;
using UnityEngine.Diagnostics;

// NOTE
// Capire come gestire l'oggetto quando viene grabbato:
// IsKinematic = true -> oltrepassa gli oggetti che non hanno rigidbody -> per evitare drop strani
//                       si puo' aggiungere un OverlapSphere o un trigger sul collider dell'oggetto
//                       per determinare quando puo' o meno essere droppato
// IsKinematic = false -> in questo caso l'oggetto interagisce con ogni collider (pareti, bancone ecc)
//                        Il problema qui e': se il player si muove con l'oggetto in mano mentre esso sbatte su un muro
//                        ad esempio, si creano situazioni spiacevoli o tremolii dell'oggetto.
//                        Un modo per risolvere potrebbe essere far si che quando l'oggetto che viene preso in mano
//                        collide con determinate superifici (non Rigidbody loro stesse magari) riconoscibili
//                        tramite LayerMask ad esempio (o altro) allora viene automaticamente droppato 
// ConfigurableJoint -> Altro metodo che permette l'interazione con gli oggetti ambientali senza causare collisioni
//                      Qualche problema se si vuole simulare "peso" dell'oggetto, ancora da verificare
// TO-DO: - linear damping e angular damping (per metodo iskinematic = false) per evitare che oggetto jitteri
//          troppo durante spostamento
//        - In generale, risolvere collisioni tra oggetto grabbato e player -> ignoreCollision volendo 

public class GrabbableItem : MonoBehaviour, IInteractable {

    [SerializeField] private bool debug; // per disattivare o meno visualizzazione dei commentii
    private Transform objectGrabPointTransform; // Posizione di grab -> assegnato per ora come child della camera nella Scena

    [SerializeField] private float itemLerpSpeed = 20f; // Velocita' di spostamento durante il grab "peso dell'oggetto"

    private Rigidbody rb;

    private ConfigurableJoint grabJoint; // Per trasporto con configurableJoint
    private void Awake() {
        rb = this.GetComponent<Rigidbody>();
    }

    public void OnInteract(PlayerInteract playerInteract) { // Quando il player ha premuto tasto di intearzione
        playerInteract.TryGrab(this); // Si richiama tryGrab, per capire se il player puo' o meno prenderlo
    }

    public void Grab(Transform grabPoint) { // Richiamato in PlayerInteract
        if (debug) Debug.Log("Grabbed : " + this.gameObject.name);
        // rb.isKinematic = true;
        rb.useGravity = false; // disattivo gravita' (metodo isKinematic = false)

        // Disabilito collisioni tra oggetto e player
        Utils.DisableCollision(gameObject.GetComponent<Collider>(), Player.Instance.gameObject.GetComponent<Collider>());

        this.objectGrabPointTransform = grabPoint; // assegno punto da seguire
    }

    private void FixedUpdate() {
        if(objectGrabPointTransform != null) {
            // Movimento di trasporto
            Vector3 desiredPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, itemLerpSpeed * Time.deltaTime);
            
            rb.MovePosition(desiredPosition);
        }
    }

    public void Release() {
        // rb.isKinematic = false;
        rb.useGravity = true; // riattivo gravita'
        this.objectGrabPointTransform = null; // ho droppato, quindi imposto a null grabpoint

        // Riabilito collisioni tra oggetto e player
        Utils.EnableCollision(gameObject.GetComponent<Collider>(), Player.Instance.gameObject.GetComponent<Collider>());

        if (debug) Debug.Log("Released : " + this.gameObject.name);
    }

    public void GrabWithJoint(Transform grabPoint) {
        // Aggiungere Release() automatico quando distanza tra Anchor e oggetto supera certa soglia!

        rb.useGravity = false;

        // ignora collisioni col player
        Utils.DisableCollision(
            GetComponent<Collider>(),
            Player.Instance.GetComponent<Collider>()
        );

        Rigidbody anchorRb = grabPoint.GetComponent<Rigidbody>();
        if (!anchorRb) {
            Debug.LogError("GrabPoint NON ha Rigidbody!");
            return;
        }

        grabJoint = gameObject.AddComponent<ConfigurableJoint>();
        grabJoint.connectedBody = anchorRb;

        grabJoint.autoConfigureConnectedAnchor = false;
        grabJoint.anchor = Vector3.zero;
        grabJoint.connectedAnchor = Vector3.zero;

        grabJoint.xMotion = ConfigurableJointMotion.Locked;
        grabJoint.yMotion = ConfigurableJointMotion.Locked;
        grabJoint.zMotion = ConfigurableJointMotion.Locked;

        grabJoint.angularXMotion = ConfigurableJointMotion.Locked;
        grabJoint.angularYMotion = ConfigurableJointMotion.Locked;
        grabJoint.angularZMotion = ConfigurableJointMotion.Locked;

        rb.freezeRotation = true;
    }

    public void ReleaseWithJoint() {
        if (grabJoint) {
            Destroy(grabJoint);
            grabJoint = null;
        }

        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.freezeRotation = false;

        Utils.EnableCollision(
            GetComponent<Collider>(),
            Player.Instance.GetComponent<Collider>()
        );

        if (debug) Debug.Log("Released : " + gameObject.name);
    }
}
