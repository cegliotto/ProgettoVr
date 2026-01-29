using UnityEngine;
using System.Collections;

public class WalkingNPC : MonoBehaviour {
    [SerializeField] private Transform[] path;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 1.5f;
    [SerializeField] private float rotationSpeed = 5f;

    private Transform targetPoint;
    private bool isWaiting;
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>(); // prendiamo l'Animator
        ChooseNextPoint();
    }

    void Update() {
        if (path.Length == 0 || targetPoint == null) return;

        if (isWaiting) {
            animator.SetBool("IsWalking", false);
            return;
        }

        Vector3 direction = (targetPoint.position - transform.position);
        direction.y = 0f;

        if (direction.magnitude > 0.05f) {
            animator.SetBool("IsWalking", true);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPoint.position,
                speed * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.05f) {
            StartCoroutine(WaitAndChooseNext());
        }
    }

    void ChooseNextPoint() {
        if (path.Length == 0) return;
        targetPoint = path[Random.Range(0, path.Length)];
    }

    private IEnumerator WaitAndChooseNext() {
        isWaiting = true;
        animator.SetBool("IsWalking", false);
        yield return new WaitForSeconds(waitTime);
        ChooseNextPoint();
        isWaiting = false;
    }
}
