//abstract class
using UnityEngine;

public abstract class PuzzleBase : MonoBehaviour
{
    //mandatory logic for each puzzle
    protected abstract void PuzzleBehaviour(); 
    protected abstract void PuzzleCompleted(); //when it is completed
    protected abstract void ExitPuzzle(); // when ESC
}