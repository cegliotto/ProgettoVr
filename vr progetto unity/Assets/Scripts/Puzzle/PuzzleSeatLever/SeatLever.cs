using UnityEngine;

public class SeatLever : MonoBehaviour
{

    [SerializeField] private float returnSpeed = 2f;   // più basso = più lento

    private bool isReturning;
    private Quaternion initialRot;
    private Quaternion downRot;

    [SerializeField] private float downAngle = 10f;

    private void Awake()
    {
        initialRot = transform.localRotation;

        // forza la rotazione in senso "verso il basso"
        downRot = initialRot * Quaternion.AngleAxis(-downAngle, Vector3.forward);
    }


    public void Pull()
    {
        if (isReturning) return;

        transform.localRotation = downRot;
        isReturning = true;
    }

    private void Update()
    {
        if (!isReturning) return;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            initialRot,
            Time.deltaTime * returnSpeed
        );

        if (Quaternion.Angle(transform.localRotation, initialRot) < 0.1f)
        {
            transform.localRotation = initialRot;
            isReturning = false;
        }
    }
}
