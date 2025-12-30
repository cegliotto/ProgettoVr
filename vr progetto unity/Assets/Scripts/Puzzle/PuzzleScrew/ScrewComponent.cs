using System.Collections;
using UnityEngine;

public class ScrewComponent : MonoBehaviour {

    [SerializeField] private float duration = 0.3f; // durata dell'animazione
    [SerializeField] private float moveStep = 0.02f; // Quanto sale per ogni click

    private int clicks = 0;

    private bool isAnimating = false; // vedi giu' per uso
    private Vector3 center; // centro della vite, serve per rotazione corretta (vedi giu')

    public bool IsRemoved { get; private set; }

    void Start() {
        // Il centro della vite, per il fatto che il pivot non coincide con il centro, lo prendo dalla mesh
        // Quindi poi devo ruotare attorno a questo centro, fare Rotate() non va bene, visto che farei attorno al pivot
        center = GetComponent<Renderer>().bounds.center;
    }

    public void RotateScrew() {
        if (isAnimating || IsRemoved) return; // In modo che non posso cliccare nuovamente se si sta gia' muovendo

        clicks++;

        // Ad ogni click ruoto e si muove verso l'alto la vite
        StartCoroutine(RotateAndLift(180f, moveStep)); // Coroutine per animazione

        if (clicks >= 2)
            IsRemoved = true; // la vite è stata completamente svitata
    }

    IEnumerator RotateAndLift(float angleTotal, float liftAmount) {
        isAnimating = true; // Animazione iniziata

        float angle = 0f; // angolo attuale di rotazione
        float moved = 0f; // spostamento attuale

        while (angle < angleTotal) {
            // calcolo angolo di rotazione attuale e spostamento
            float stepAngle = (angleTotal / duration) * Time.deltaTime;
            float stepLift = (liftAmount / duration) * Time.deltaTime;

            // Per evitare di sforare
            if (angle + stepAngle > angleTotal)
                stepAngle = angleTotal - angle;

            transform.RotateAround(center, Vector3.up, stepAngle); // Rotazione della vite
            transform.Translate(Vector3.up * stepLift, Space.Self); // Salita della vite

            angle += stepAngle;
            moved += stepLift;

            yield return null;
        }

        isAnimating = false; // Animazione finita

        // Se ho finito l'animazione ed e' il secondo click -> rimozione
        if (IsRemoved)
            gameObject.SetActive(false);
    }
}
