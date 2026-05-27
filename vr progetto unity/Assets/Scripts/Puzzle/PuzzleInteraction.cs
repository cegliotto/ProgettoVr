using UnityEngine;

public enum PuzzleType {
    PuzzleCabinet,
    PuzzleDoor,
    PuzzleNecklace,
    PuzzleScrews,
    PuzzleSeatLever
}

// used for user interactions
public class PuzzleInteraction : MonoBehaviour, IInteractable
{
    [Tooltip("Next Scene Name")]
    //[SerializeField] protected string puzzleSceneName;
    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private AudioListener puzzleListener;
    [SerializeField] private GameObject puzzleObj;
    [SerializeField] protected PuzzleType puzzleType; // which puzzle
    [SerializeField] public GlowUntilSolved glow; 

    public bool solved = false;
    public bool unlocked;
    [SerializeField] protected Animator animator;
    public PuzzleType GetPuzzleType() => puzzleType;

    private AudioSource audioSource;
    [SerializeField] private bool playSound;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (glow != null){ glow.enabled = false; }

        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Start() {
        if (PuzzleManager.Instance != null) {
            if (PuzzleManager.Instance.isPuzzleUnlocked(this.puzzleType)) {
                unlocked = true;
            }
            if (PuzzleManager.Instance.isPuzzleSolved(this.puzzleType)) { 
                StartSolvedAnimation(); // puzzle solved
            }
        }
    }

    public virtual void OnInteract(PlayerInteract playerInteract) {
        if (!enabled) return;
        if (solved) return;

        if (PuzzleManager.Instance != null) {
            PuzzleManager.Instance.StartPuzzle(puzzleObj, puzzleCamera, puzzleListener , puzzleType); // update the related scene
        }
        else {
            Debug.Log("Puzzle manager not found");
        }
    }

    public virtual void StartSolvedAnimation() {
        if(animator != null) { // if there is an animation after solved
            animator.SetTrigger("completed"); 
        }

        this.solved = true; 
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        if(playSound && audioSource != null) {
            audioSource.Play();
        }

        // so as not to block the grip of other objects
        int layer = LayerMask.NameToLayer("Ignore Raycast");
        if (layer != -1) {
            gameObject.layer = layer;
        }
    }
}