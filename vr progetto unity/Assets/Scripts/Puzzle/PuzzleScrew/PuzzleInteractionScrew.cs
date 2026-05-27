// communication between objects
using UnityEngine;

public class PuzzleInteractionScrew : PuzzleInteraction, IInteractable
{
    [SerializeField] private GameObject movableNightTable; // Night table that can be moved
    [SerializeField] private GameObject necklace;

    protected override void Start()
    {
        base.Start();
    }

    public override void OnInteract(PlayerInteract playerInteract)
    {
        base.OnInteract(playerInteract);
    }

    public override void StartSolvedAnimation()
    {
        base.StartSolvedAnimation(); 

        if (movableNightTable != null)
        {
            // Forces the activation of the night table
            movableNightTable.SetActive(true);

            // Gets the PhysicalUnlock component and starts the animation
            if (movableNightTable.TryGetComponent<PhysicalUnlock>(out PhysicalUnlock unlock))
            {
                unlock.Unlock();
            }
        }
        if (necklace != null)
        {
            necklace.SetActive(true);
        }

        // Deactivates the interactable screws in the main scene
        // this.gameObject.SetActive(false);
    }
}