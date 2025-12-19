using UnityEngine;

public abstract class PuzzleBase : MonoBehaviour
{
    protected abstract void PuzzleBehaviour();
    protected abstract void PuzzleCompleted(/* item da passare*/);
}
