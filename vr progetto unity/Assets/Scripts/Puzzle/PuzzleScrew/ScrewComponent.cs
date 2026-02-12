using System.Collections;
using UnityEngine;

public class ScrewComponent : MonoBehaviour
{

    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float moveStep = 0.02f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip screwRotateClip;
    [SerializeField] private AudioClip screwRemovedClip;

    private int clicks = 0;
    private bool isAnimating = false;
    private Vector3 center;

    public bool IsRemoved { get; private set; }

    void Start()
    {
        center = GetComponent<Renderer>().bounds.center;

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    public void RotateScrew()
    {
        if (isAnimating || IsRemoved) return;

        clicks++;

        // gestione audio
        if (clicks >= 2)
        {
            IsRemoved = true;
            if (audioSource && screwRemovedClip)
                audioSource.PlayOneShot(screwRemovedClip);
        }
        else
        {
            if (audioSource && screwRotateClip)
                audioSource.PlayOneShot(screwRotateClip);
        }

        // avvio animazione
        StartCoroutine(RotateAndLift(180f, moveStep));
    }

    IEnumerator RotateAndLift(float angleTotal, float liftAmount)
    {
        isAnimating = true;

        float angle = 0f;
        float moved = 0f;

        while (angle < angleTotal)
        {
            float stepAngle = (angleTotal / duration) * Time.deltaTime;
            float stepLift = (liftAmount / duration) * Time.deltaTime;

            if (angle + stepAngle > angleTotal)
                stepAngle = angleTotal - angle;

            transform.RotateAround(center, Vector3.up, stepAngle);
            transform.Translate(Vector3.up * stepLift, Space.Self);

            angle += stepAngle;
            moved += stepLift;

            yield return null;
        }

        isAnimating = false;

        if (IsRemoved)
        {
            // Aspetta un istante per far finire il suono prima di disattivare l'oggetto
            yield return new WaitForSeconds(0.2f); // Leggermente aumentato per sicurezza
            gameObject.SetActive(false);
        }
    }
}