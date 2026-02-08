using UnityEngine;

public class PuzzleInteractionScrew : PuzzleInteraction, IInteractable
{
    [SerializeField] private GameObject movableNightTable; // Night table che puo' essere spostato
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
        base.StartSolvedAnimation(); // Imposta solved = true

        if (movableNightTable != null)
        {
            // Forza l'attivazione del comodino se necessario
            movableNightTable.SetActive(true);

            // CERCA lo script PhysicalUnlock e avvia l'animazione
            if (movableNightTable.TryGetComponent<PhysicalUnlock>(out PhysicalUnlock unlock))
            {
                unlock.Unlock();
            }
        }
        if (necklace != null)
        {
            necklace.SetActive(true);
        }

        // Disattiva le viti interagibili nella scena principale
        // this.gameObject.SetActive(false);
    }
}