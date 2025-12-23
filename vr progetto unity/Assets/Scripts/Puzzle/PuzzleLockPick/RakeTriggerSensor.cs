using UnityEngine;

public class RakeTriggerSensor : MonoBehaviour
{
    [SerializeField] private PuzzleLockpick puzzle;

    private void OnTriggerEnter(Collider other) => puzzle.SetZone(other);
    private void OnTriggerStay(Collider other) => puzzle.SetZone(other);
    private void OnTriggerExit(Collider other) => puzzle.ClearZone(other);
}
