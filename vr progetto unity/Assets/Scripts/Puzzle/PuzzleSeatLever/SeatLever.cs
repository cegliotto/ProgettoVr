using UnityEngine;
using System;

public class SeatLever : MonoBehaviour
{
    [SerializeField] private float returnSpeed = 2f;
    [SerializeField] private float downAngle = 1f;
    [SerializeField] private AudioSource leverAudio;
    [SerializeField] private AudioClip leverClick;


    private bool isReturning;
    private Quaternion initialRot;
    private Quaternion downRot;

    // EVENTO
    public event Action OnLeverPulled;

    private void Awake()
    {
        initialRot = transform.localRotation;
        downRot = initialRot * Quaternion.AngleAxis(-downAngle, Vector3.forward);
    }

    public void Pull()
    {
        if (isReturning) return;

        transform.localRotation = downRot;
        isReturning = true;
        if (leverAudio && leverClick)
            leverAudio.PlayOneShot(leverClick);
        // UNA SOLA NOTIFICA PER PULL
        OnLeverPulled?.Invoke();
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
