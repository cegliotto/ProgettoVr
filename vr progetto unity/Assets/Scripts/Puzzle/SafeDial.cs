using UnityEngine;

public class SafeDial : MonoBehaviour {
    public int CurrentValue { get; private set; }

    [SerializeField] private float rotationPerStep = 9f;
    [SerializeField] private int modulo = 40;

    private float accumulated; // SIGNED
    [SerializeField] private Transform dial;

    public void Rotate(float input) {
        // input è una rotazione in gradi (positiva o negativa)
        dial.Rotate(0f, 0f, -input, Space.Self);

        accumulated += input; // NOT abs

        while (Mathf.Abs(accumulated) >= rotationPerStep) {
            int dir = accumulated > 0f ? 1 : -1;

            CurrentValue = (CurrentValue + dir + modulo) % modulo;

            accumulated -= dir * rotationPerStep;
        }
    }
}
