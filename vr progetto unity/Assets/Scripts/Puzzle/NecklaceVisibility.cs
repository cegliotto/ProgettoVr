using UnityEngine;

public class NecklaceVisibility : MonoBehaviour
{
    void Start()
    {
        // checks if the PuzzleManager exists in the scene
        if (PuzzleManager.Instance != null)
        {
            // checks if the screws puzzle has been solved
            bool isSolved = PuzzleManager.Instance.isPuzzleSolved(PuzzleType.PuzzleScrews);

            // activates or deactivates the necklace based on the puzzle state
            gameObject.SetActive(isSolved);
        }
        else
        {
            // manager not found -> hidden object
            gameObject.SetActive(false);
        }
    }
}