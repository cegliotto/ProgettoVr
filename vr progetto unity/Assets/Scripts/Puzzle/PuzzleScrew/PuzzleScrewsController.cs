using UnityEngine;

public class PuzzleScrewsController : PuzzleBase
{
    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private ScrewComponent[] screws;

    private ScrewComponent selectedScrew;

    void Update()
    {
        PuzzleBehaviour();
        ExitPuzzle();
    }

    protected override void PuzzleBehaviour()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseInteraction();
        }
    }

    private void HandleMouseInteraction()
    {
        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ScrewComponent clickedScrew = hit.collider.GetComponent<ScrewComponent>();

            if (clickedScrew != null)
            {
                if (selectedScrew != clickedScrew)
                {
                    selectedScrew = clickedScrew;
                }

                selectedScrew.RotateScrew();
                CheckCompletion();
            }
        }
    }

    private void CheckCompletion()
    {
        foreach (var s in screws)
        {
            if (!s.IsRemoved) return;
        }

        PuzzleCompleted();
    }

    protected override void PuzzleCompleted()
    {
        PuzzleManager.Instance?.CompletePuzzle();
    }

    protected override void ExitPuzzle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PuzzleManager.Instance?.ExitFromPuzzle();
        }
    }
}