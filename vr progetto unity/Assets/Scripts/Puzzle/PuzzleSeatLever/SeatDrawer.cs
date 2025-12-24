using UnityEngine;

public class SeatDrawer : MonoBehaviour
{

    [SerializeField] private float maxOpenDistance = 10f;
    [SerializeField] private float openStep = 1f;
    [SerializeField] private float closeSpeed = 0.25f;

    private Vector3 closedPos;
    private float currentOpen;

    private void Awake()
    {
        closedPos = transform.localPosition;
    }

    public void OpenStep()
    {
        currentOpen = Mathf.Clamp(currentOpen + openStep, 0f, maxOpenDistance);
        UpdatePosition();
    }

    public void CloseStep(float deltaTime)
    {
        currentOpen = Mathf.Clamp(currentOpen - closeSpeed * deltaTime, 0f, maxOpenDistance);
        UpdatePosition();
    }

    public bool IsFullyOpen()
    {
        return currentOpen >= maxOpenDistance * 0.95f;
    }

    private void UpdatePosition()
    {
        transform.localPosition = closedPos - Vector3.right * currentOpen;
    }
}
