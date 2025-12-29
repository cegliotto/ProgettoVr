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
        // Ora gestiamo tutto all'interno del click del mouse
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
            // Debug per vedere cosa stai colpendo effettivamente
            Debug.Log("Ho colpito: " + hit.collider.gameObject.name);

            ScrewComponent clickedScrew = hit.collider.GetComponent<ScrewComponent>();

            if (clickedScrew != null)
            {
                // Se clicchi una vite diversa, la selezioni
                if (selectedScrew != clickedScrew)
                {
                    selectedScrew = clickedScrew;
                    Debug.Log("Nuova vite selezionata: " + selectedScrew.name);
                }

                // Applica la rotazione (sia se è appena stata selezionata, sia se era già selezionata)
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