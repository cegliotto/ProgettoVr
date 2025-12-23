using UnityEngine;

public class ScrewComponent : MonoBehaviour
{
    private int clicks = 0;
    public bool IsRemoved { get; private set; }

    public void RotateScrew()
    {
        if (IsRemoved) return;

        clicks++;
        transform.Rotate(0, 0, 180f); // Mezzo giro

        if (clicks >= 2)
        {
            IsRemoved = true;
            gameObject.SetActive(false); // La vite scompare
        }
    }
}