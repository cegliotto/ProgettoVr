using UnityEngine;

public class BarMan : MonoBehaviour {

    private const string IDLE = "Idle";
    private const string TALKING = "Talking";
    private const string CLEANING = "CleaningFloor";
    private const string WALKING = "Walk";
    private const string STRETCHING = "Stretching";
    private const string DWARF_IDLE = "Dwarf_idle";
    private const string LOOK_AROUND = "Look_around";

    public string currentAnimation;

    [SerializeField] private float speed = 2f; 
    [SerializeField] private float rotationSpeed = 5f; 
    [SerializeField] private float idleDecisionTime = 3f;
    [SerializeField] private Transform[] locations;

    [SerializeField] private GameObject cleaningBroom;

    private Transform destinationLocation;
    private Animator anim;
    private float idleTimer;
    private Quaternion initialRotation;

    public bool isTalking;

    private void Awake() {
        if (locations.Length > 0)
            transform.position = new Vector3(locations[0].position.x, transform.position.y, locations[0].position.z);

        anim = GetComponent<Animator>();

        initialRotation = transform.rotation;
        currentAnimation = IDLE;

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) { // Per test
            SetTalking(!isTalking);
        }

        if (isTalking) {
            Talking();
            return;
        }

        switch (currentAnimation) {
            case IDLE: Idle(); break;
            case WALKING: Moving(); break;
            case CLEANING: Cleaning(); break;
            case STRETCHING: Stretching(); break;
            case DWARF_IDLE: DwarfIdle(); break;
            case LOOK_AROUND: LookAround(); break;
        }
    }

    public void SetTalking(bool value) {
        isTalking = value;
        cleaningBroom.SetActive(false); // tolgo la scopa

        if (isTalking) {
            idleTimer = 0f;
            ChangeAnimation(TALKING, 0.05f);
        }
        else {
            ChangeAnimation(IDLE, 0.1f);
        }
    }

    private void ChangeAnimation(string animation, float crossFade = 0.2f) {
        if (currentAnimation != animation) {
            currentAnimation = animation;
            anim.CrossFade(animation, crossFade);
        }
    }

    private void Talking() {

        Vector3 lookTarget = new Vector3(
            Player.Instance.transform.position.x,
            transform.position.y,
            Player.Instance.transform.position.z
        );

        Vector3 direction = (lookTarget - transform.position).normalized;

        if (direction.sqrMagnitude > 0.001f) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }


    private void Idle() {
        cleaningBroom.SetActive(false);
        idleTimer += Time.deltaTime;

        transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, rotationSpeed * Time.deltaTime);

        if (idleTimer >= idleDecisionTime) {
            idleTimer = 0f;
            DecideNextState();
        }
    }

    private void Moving() {
        Vector3 targetPos = new Vector3(destinationLocation.position.x, transform.position.y, destinationLocation.position.z);
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance > 0.1f) {

            Vector3 destination = new Vector3(
                destinationLocation.position.x,
                transform.position.y,
                destinationLocation.position.z
            );

            Vector3 direction = (destination - transform.position).normalized;

            transform.position += direction * speed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else {
            ChangeAnimation(IDLE, 0.01f);
        }
    }

    private void Cleaning() {
        //
    }

    public void OnCleaningFinished() { // evento definito nell'animation clip!!
        cleaningBroom.SetActive(false);
        ChangeAnimation(IDLE, 0.05f);
    }
    private void Stretching() {
        //
    }
    public void OnArmStretchingFinished() { // evento definito nell'animation clip!!
        ChangeAnimation(IDLE);
    }
    private void DwarfIdle() {
        //
    }
    public void OnDwarfIdleFinished() { // evento definito nell'animation clip!!
        ChangeAnimation(IDLE);
    }
    private void LookAround() {
        //
    }
    public void OnLookAroundFinished() { // evento definito nell'animation clip!!
        ChangeAnimation(IDLE);
    }
    private void DecideNextState() {
        if (isTalking) return;

        int choice = Random.Range(0, 9);

        switch (choice) {
            case 0:
                ChangeAnimation(CLEANING);
                cleaningBroom.SetActive(true);
                break;
            case 1:
            case 2:
                PickRandomDestination();
                ChangeAnimation(WALKING);
                break;
            case 3:
            case 4:
                ChangeAnimation(STRETCHING);
                break;
            case 5:
            case 6:
                ChangeAnimation(DWARF_IDLE);
                break;
            case 7:
            case 8:
                ChangeAnimation(LOOK_AROUND);
                break;
        }

    }

    private void PickRandomDestination() {
        if (locations.Length == 0) return;
        int attempts = 0;
        Transform newDest;
        do {
            newDest = locations[Random.Range(0, locations.Length)];
            attempts++;
        } while (newDest == destinationLocation && attempts < 10);

        destinationLocation = newDest;
    }
}