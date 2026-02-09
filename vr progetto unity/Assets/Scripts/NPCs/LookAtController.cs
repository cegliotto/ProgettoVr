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

        objectLookAt = Player.Instance.transform;
    }

    private void OnAnimatorIK(int layerIndex) {
        if (Player.Instance == null) return;

        if(Player.Instance.playerState == Player.PlayerState.Dialog) {
            animator.SetLookAtPosition(objectLookAt.position);
            animator.SetLookAtWeight(1, bodyWeight, headWeight);
        }
    }
}
