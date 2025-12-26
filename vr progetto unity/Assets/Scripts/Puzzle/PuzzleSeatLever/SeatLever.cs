using UnityEngine;

public class SeatLever : MonoBehaviour
{

    [SerializeField] private float returnSpeed = 2f;   // più basso = più lento

    private bool isReturning;
    private Quaternion initialRot;
    private Quaternion downRot;

    [SerializeField] private float downAngle = 1f;

    private void Awake()
    {
        initialRot = transform.localRotation;
        Debug.Log("Initial rotation in Euler angles: " + initialRot.eulerAngles);



        // forza la rotazione in senso "verso il basso"
        downRot = initialRot * Quaternion.AngleAxis(-downAngle, Vector3.forward);
    }


    public void Pull()
    {
        if (isReturning) return;

        transform.localRotation = downRot;
        Debug.Log("Second rotation in Euler angles: " + downRot.eulerAngles);
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
