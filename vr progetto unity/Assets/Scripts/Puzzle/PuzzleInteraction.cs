using UnityEngine;

public enum PuzzleType {
    PuzzleCabinet,
    PuzzleDoor,
    PuzzleNecklace,
    PuzzleScrews,
}

// Script usato per l'interazione con il player
public class PuzzleInteraction : MonoBehaviour, IInteractable
{
    [Tooltip("Nome della scena da carica per far partire il puzzle specifico")]
    [SerializeField] private string puzzleSceneName;
    [SerializeField] private PuzzleType puzzleType; // Necessario per identificare il puzzle

    public bool solved = false;
    private Animator animator;
    public PuzzleType GetPuzzleType() => puzzleType;

    private void Awake() {
        if(TryGetComponent<Animator>(out Animator anim)){
            animator = anim;
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
        this.solved = true; // lo segno come completato

        // Una volta completato assegno al puzzle il layer noInteractable
        // in modo che, ad esempio per la cabinet, il raycast non venga bloccato dalla cabinet quando si cerca
        // di prendere la borsa
        int layer = LayerMask.NameToLayer("NoInteractable");
        if (layer != -1) {
            gameObject.layer = layer;
        }
    }
}
