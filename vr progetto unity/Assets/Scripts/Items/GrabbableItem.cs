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

    [SerializeField] AudioSource source;
    private Rigidbody rb;

    private ConfigurableJoint grabJoint; // Per trasporto con configurableJoint

    private Vector3 localCenter;

    private void Awake() {
        rb = this.GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();

        if(TryGetComponent<Collider>(out Collider col)) {
            Vector3 worldCenter = col.bounds.center;
            localCenter = transform.InverseTransformPoint(worldCenter);
        }
        else {
            localCenter = Vector3.zero; // coincide con pivot
        }
    }

    public void OnInteract(PlayerInteract playerInteract) { // Quando il player ha premuto tasto di intearzione
        playerInteract.TryGrab(this); // Si richiama tryGrab, per capire se il player puo' o meno prenderlo
    }

    public void Grab(Transform grabPoint) { // Richiamato in PlayerInteract
        if (debug) Debug.Log($"Grabbed : {gameObject.name} in {grabPoint.gameObject.name}");
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


    // Variabili interne per salvare lo stato originale dell'oggetto grabbable
    private float _originalLinearDamping;
    private float _originalAngularDamping;
    float originalMass;
    private RigidbodyInterpolation _originalInterpolation;

    public void GrabWithJoint(Transform grabPoint) {
        originalMass = rb.mass;
        rb.mass = 0.1f; // Massa leggera per non spingere via oggetti pesanti

        rb.useGravity = false;

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.freezeRotation = false;

        _originalInterpolation = rb.interpolation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        _originalLinearDamping = rb.linearDamping;
        _originalAngularDamping = rb.angularDamping;


        rb.linearDamping = 20f;  
        rb.angularDamping = 20f; 

        Utils.DisableCollision(GetComponent<Collider>(), Player.Instance.GetComponent<Collider>());

        Rigidbody anchorRb = grabPoint.GetComponent<Rigidbody>();
        if (!anchorRb) {
            Debug.LogError("GrabPoint NON ha Rigidbody!");
            return;
        }

        grabJoint = gameObject.AddComponent<ConfigurableJoint>();
        grabJoint.connectedBody = anchorRb;
        grabJoint.autoConfigureConnectedAnchor = false;
        // grabJoint.anchor = Vector3.zero; // presa nel pivot
        grabJoint.anchor = localCenter; // presa al centro dell'oggetto
        grabJoint.connectedAnchor = Vector3.zero;

        grabJoint.projectionMode = JointProjectionMode.None;

        grabJoint.xMotion = ConfigurableJointMotion.Limited;
        grabJoint.yMotion = ConfigurableJointMotion.Limited;
        grabJoint.zMotion = ConfigurableJointMotion.Limited;

        grabJoint.linearLimit = new SoftJointLimit { limit = 0.001f };

        grabJoint.linearLimitSpring = new SoftJointLimitSpring {
            spring = 200f,
            damper = 20f
        };

        grabJoint.angularXMotion = ConfigurableJointMotion.Free;
        grabJoint.angularYMotion = ConfigurableJointMotion.Free;
        grabJoint.angularZMotion = ConfigurableJointMotion.Free;

        grabJoint.rotationDriveMode = RotationDriveMode.Slerp;

        grabJoint.slerpDrive = new JointDrive {
            positionSpring = 3000f, 
            positionDamper = 100f, 
            maximumForce = Mathf.Infinity
        };

        if (debug) Debug.Log($"GRABBED: {name}");
    }

    public void ReleaseWithJoint() {
        if (grabJoint) {
            Destroy(grabJoint);
            grabJoint = null;
        }

        rb.useGravity = true;
        rb.mass = originalMass;

        rb.interpolation = _originalInterpolation;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        rb.linearDamping = _originalLinearDamping;
        rb.angularDamping = _originalAngularDamping;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Utils.EnableCollision(GetComponent<Collider>(), Player.Instance.GetComponent<Collider>());

        if (debug) Debug.Log("Released : " + gameObject.name);
    }

    
    void OnCollisionEnter(Collision collision)
    {
        if(source !=null && !source.isPlaying)
        {
            source.Play();
        }
    }
}
