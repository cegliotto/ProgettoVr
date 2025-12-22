using UnityEngine;

public class SeatLever : MonoBehaviour
{

    [SerializeField] private float downAngle = -45f;
    [SerializeField] private float returnSpeed = 6f;

    private bool isPulled;
    private Quaternion initialRot;

    private void Awake()
    {
        initialRot = transform.localRotation;
    }

    public void Pull()
    {
        if (isPulled) return;

        isPulled = true;
        transform.localRotation = Quaternion.Euler(downAngle, 0, 0);
    }

    private void Update()
    {
        if (!isPulled) return;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            initialRot,
            Time.deltaTime * returnSpeed
        );

        if (Quaternion.Angle(transform.localRotation, initialRot) < 0.5f)
            isPulled = false;
    }
}
