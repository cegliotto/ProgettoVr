using UnityEngine;

public class SafeDial : MonoBehaviour {

    public int CurrentValue { get; private set; }

    [SerializeField] private float rotationPerStep = 9f; // Step di rotazione per passare a numero successivo
    [SerializeField] private int maxDigit = 40;
    [SerializeField] private Transform dial;

    private float accumulated;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tickClip;

    private float minTimeBetweenTicks = 0.05f;
    private float lastTickTime;

    public void Rotate(float input) {

        accumulated += input;

        while (Mathf.Abs(accumulated) >= rotationPerStep) {
            int dir = accumulated > 0f ? 1 : -1; // Direzione di rotazione
            CurrentValue = (CurrentValue + dir + maxDigit) % maxDigit; // modulo in modo da andare a 39 se valore negativo
            dial.Rotate(0f, 0f, -dir * rotationPerStep, Space.Self); // rotazione attorno ad asse z locale

            if (audioSource && tickClip) {
                PlayTick();
            }

            accumulated -= dir * rotationPerStep;
        }
    }

    void PlayTick() {
        if (Time.time - lastTickTime < minTimeBetweenTicks)
            return;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(tickClip);
        lastTickTime = Time.time;
    }
}
