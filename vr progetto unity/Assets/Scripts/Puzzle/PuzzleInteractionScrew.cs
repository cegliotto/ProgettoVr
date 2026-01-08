using UnityEngine;

public class PuzzleInteractionScrew : PuzzleInteraction, IInteractable
{
    [SerializeField] private GameObject movableNightTable; // Night table che puo' essere spostato

    protected override void Start() {
        base.Start();
    }

    public override void OnInteract(PlayerInteract playerInteract) {
        base.OnInteract(playerInteract);
    }

    public override void StartSolvedAnimation() {
        base.StartSolvedAnimation();

        movableNightTable.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
