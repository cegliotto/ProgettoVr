using UnityEngine;

public class LeverDrawerLink : MonoBehaviour
{
    [SerializeField] private SeatLever lever;
    [SerializeField] private SeatDrawer drawer;

    private void OnEnable()
    {
        if (lever != null)
            lever.OnLeverPulled += drawer.OpenStep;
    }

    private void OnDisable()
    {
        if (lever != null)
            lever.OnLeverPulled -= drawer.OpenStep;
    }
}
