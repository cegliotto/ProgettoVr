using UnityEngine;

public class ScrewComponent : MonoBehaviour
{
    private int clicks = 0;
    public bool IsRemoved { get; private set; }

    public void RotateScrew()
    {
        if (IsRemoved) return;

        clicks++;
        transform.Rotate(0, 0, 180f); // mezzo giro

        if (clicks >= 2) //dopo due click (=2 mezzi giri) 
        {
            IsRemoved = true; //la vite è rimossa 
            gameObject.SetActive(false); // la vite scompare
        }
    }
}