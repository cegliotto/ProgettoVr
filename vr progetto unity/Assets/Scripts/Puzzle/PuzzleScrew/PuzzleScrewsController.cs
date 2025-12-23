using UnityEngine;

public class PuzzleScrewsController : PuzzleBase
{
    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private ScrewComponent[] screws; // Array di viti

    private ScrewComponent selectedScrew;

    void Update()
    {
        PuzzleBehaviour(); // Metodo richiesto da PuzzleBase
        ExitPuzzle();      // Metodo richiesto da PuzzleBase
    }

    protected override void PuzzleBehaviour()
    {
        // Selezione vite con Mouse
        if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }

        // Azione con tasto W (come da tua richiesta)
        if (selectedScrew != null && Input.GetKeyDown(KeyCode.W))
        {
            selectedScrew.RotateScrew();
            CheckCompletion();
        }
    }

    private void HandleSelection()
    {
        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            selectedScrew = hit.collider.GetComponent<ScrewComponent>();
        }
    }

    private void CheckCompletion()
    {
        foreach (var s in screws)
        {
            if (!s.IsRemoved) return;
        }
        PuzzleCompleted(); // Metodo di PuzzleBase che richiama il Manager
    }

    protected override void PuzzleCompleted()
    {
        PuzzleManager.Instance?.CompletePuzzle(); // Torna al treno e sblocca l'animazione
    }

    protected override void ExitPuzzle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PuzzleManager.Instance?.ExitFromPuzzle(); // Esce senza salvare il completamento
        }
    }
}