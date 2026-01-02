using UnityEngine;

public class SeatDrawer : MonoBehaviour
{

    [SerializeField] private float maxOpenDistance = 10f;
    [SerializeField] private float openStep = 1f;
    [SerializeField] private float closeSpeed = 0.25f;
    [SerializeField] private AudioSource drawerAudio;
    [SerializeField] private AudioClip drawerSlide;
    [SerializeField] private float soundStartOffset = 0.5f; // in secondi


    private Vector3 closedPos;
    private float currentOpen;
    private bool isLockedOpen = false;

    private void Awake()
    {
        closedPos = transform.localPosition;
    }

    public void OpenStep()
    {
        if (isLockedOpen) return;
        currentOpen = Mathf.Clamp(currentOpen + openStep, 0f, maxOpenDistance);
        UpdatePosition();
        // suono cassetto
        if (drawerAudio && drawerSlide)
            {
            drawerAudio.clip = drawerSlide;
            drawerAudio.time = soundStartOffset; // parte dopo mezzo secondo
            drawerAudio.Play();
        }
        // Blocca la chiusura se il cassetto è completamente aperto
        if (currentOpen >= maxOpenDistance)
        {
            isLockedOpen = true;
        }
    }

    public void CloseStep(float deltaTime)
    {
        if (isLockedOpen) return;
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
