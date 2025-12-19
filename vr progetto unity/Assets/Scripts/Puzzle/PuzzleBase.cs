using UnityEngine;

public abstract class PuzzleBase : MonoBehaviour
{
    protected abstract void PuzzleBehaviour(); // Comportamento del puzzle
    protected abstract void PuzzleCompleted(/* item da passare*/);
    protected abstract void ExitPuzzle(); // Uscire dal puzzle non ancora completato
}
