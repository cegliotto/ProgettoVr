using UnityEngine;
using UnityEngine.InputSystem;

public class DebugShortcut : MonoBehaviour
{
    [SerializeField] private PuzzleInteraction[] puzzles;
    [SerializeField] private bool active;

    // Update is called once per frame
    void Update() {
        if (!active) return;
        // premendo U sblocco tutti i puzzles
        if (Keyboard.current.uKey.wasPressedThisFrame) {
            foreach (PuzzleInteraction puzzle in puzzles) {
                puzzle.unlocked = true;
            }
        }
    }
}
