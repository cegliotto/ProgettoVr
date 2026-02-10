using UnityEngine;

public class LookAtController : MonoBehaviour
{
    public Transform objectLookAt;
    [SerializeField] private float headWeight;
    [SerializeField] private float bodyWeight;
    private Animator animator;
    [SerializeField] private float minDistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        if (Player.Instance == null) return;

        objectLookAt = Player.Instance.transform.Find("CameraPosition");
    }

    private void OnAnimatorIK(int layerIndex) {
        if (Player.Instance == null || objectLookAt == null) return;

        Vector3 dirToPlayer = Player.Instance.transform.position - transform.position;
        dirToPlayer.y = 0f;
        float angle = Vector3.Angle(transform.forward, dirToPlayer); // Per evitare che giri la testa tipo gufo

        if (Player.Instance.playerState == Player.PlayerState.Dialog && angle < 70f) {
            animator.SetLookAtPosition(objectLookAt.position);
            animator.SetLookAtWeight(1, bodyWeight, headWeight);
        }
        else {
            animator.SetLookAtWeight(0f);
        }
    }
}
