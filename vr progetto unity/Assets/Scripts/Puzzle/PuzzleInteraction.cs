using UnityEngine;


public enum PuzzleType {
    PuzzleCabinet,
    PuzzleDoor,
    PuzzleNecklace,
    PuzzleScrews,
    PuzzleSeatLever
}

// Script usato per l'interazione con il player
public class PuzzleInteraction : MonoBehaviour, IInteractable
{
    [Tooltip("Nome della scena da carica per far partire il puzzle specifico")]
    [SerializeField] private string puzzleSceneName;
    [SerializeField] private PuzzleType puzzleType; // Necessario per identificare il puzzle
    

    public bool solved = false;
    [SerializeField] private Animator animator;
    public PuzzleType GetPuzzleType() => puzzleType;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start() {
        if (PuzzleManager.Instance != null) {
            if (PuzzleManager.Instance.isPuzzleSolved(this.puzzleType)) { // Se puzzle e' gia' risolto
                StartSolvedAnimation(); // Imposto come risolto
            }
        }
    }

    public void OnInteract(PlayerInteract playerInteract) {
        if (solved) return;

        Debug.Log($"Interazione con {puzzleSceneName}");

        if (PuzzleManager.Instance != null) {
            PuzzleManager.Instance.StartPuzzle(puzzleSceneName, puzzleType); // Carico la scena relativa a questo puzzle
        }
        else {
            Debug.Log("Puzzle manager non trovato");
        }
    }

    public void StartSolvedAnimation() {
        if(animator != null) { // Se ha un animazione associata al completamento
            animator.SetTrigger("completed"); // Necessario che animazione si chiami completed in caso
        }

        //attivo solo se l'oggetto ha lo script PhysicalUnlock attivato 
        if(TryGetComponent < PhysicalUnlock> (out PhysicalUnlock unlock))
        {
            unlock.Unlock();
        }

        this.solved = true; // lo segno come completato

        if (puzzleType == PuzzleType.PuzzleScrews) return; // La nightTable deve essere spostabile

        // Una volta completato assegno al puzzle il layer Ignore Raycast
        // in modo che, ad esempio per la cabinet, il raycast non venga bloccato dalla cabinet quando si cerca
        // di prendere la borsa
        int layer = LayerMask.NameToLayer("Ignore Raycast");
        if (layer != -1) {
            gameObject.layer = layer;
        }
    }
}
