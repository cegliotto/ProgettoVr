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
            ScrewComponent clickedScrew = hit.collider.GetComponent<ScrewComponent>();

            if (clickedScrew != null)
            {

                if (selectedScrew == clickedScrew) //controllo che vite ho selezionato 
                {
                    // È la stessa vite quindi ruota
                    selectedScrew.RotateScrew();
                    CheckCompletion();
                }
                else
                {
                    // È una nuova vite: selezionala
                    selectedScrew = clickedScrew;
                    Debug.Log("Vite selezionata: " + selectedScrew.name);
                }
            }
        }
    }

    private void HandleSelection() //verifica che sia stato colpito qualcosa con un collider 
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