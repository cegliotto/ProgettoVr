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
    //[SerializeField] protected string puzzleSceneName;
    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private GameObject puzzleObj;
    [SerializeField] protected PuzzleType puzzleType; // Necessario per identificare il puzzle
    

    public bool solved = false;
    public bool unlocked;
    [SerializeField] protected Animator animator;
    public PuzzleType GetPuzzleType() => puzzleType;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    protected virtual void Start() {
        if (PuzzleManager.Instance != null) {
            if (PuzzleManager.Instance.isPuzzleUnlocked(this.puzzleType)) {
                unlocked = true;
            }
            if (PuzzleManager.Instance.isPuzzleSolved(this.puzzleType)) { // Se puzzle e' gia' risolto
                StartSolvedAnimation(); // Imposto come risolto
            }
        }
    }

    public virtual void OnInteract(PlayerInteract playerInteract) {
        if (!enabled) return;
        if (solved) return;

        //Debug.Log($"Interazione con {puzzleSceneName}");

        if (PuzzleManager.Instance != null) {
            PuzzleManager.Instance.StartPuzzle(puzzleObj, puzzleCamera, puzzleType); // Carico la scena relativa a questo puzzle
        }
        else {
            Debug.Log("Puzzle manager non trovato");
        }
    }

    public virtual void StartSolvedAnimation() {
        int oldLayer = 0;
        if(animator != null) { // Se ha un animazione associata al completamento
            animator.SetTrigger("completed"); // Necessario che animazione si chiami completed in caso
            gameObject.layer = oldLayer;
        }

        this.solved = true; // lo segno come completato

        // Una volta completato assegno al puzzle il layer Ignore Raycast
        // in modo che, ad esempio per la cabinet, il raycast non venga bloccato dalla cabinet quando si cerca
        // di prendere la borsa
        int layer = LayerMask.NameToLayer("Ignore Raycast");
        if (layer != -1) {
            oldLayer = gameObject.layer;
            gameObject.layer = layer;
        }
    }
}
