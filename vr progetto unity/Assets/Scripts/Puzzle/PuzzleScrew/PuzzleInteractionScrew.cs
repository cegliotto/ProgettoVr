using UnityEngine;

public class PuzzleInteractionScrew : PuzzleInteraction, IInteractable
{
    [SerializeField] private GameObject movableNightTable;
    [SerializeField] private GameObject necklace; 

    public override void StartSolvedAnimation()
    {
        base.StartSolvedAnimation(); // Imposta solved = true

        // Attiva la collana se è stata assegnata
        if (necklace != null)
        {
            necklace.SetActive(true);
        }

        if (movableNightTable != null)
        {
            movableNightTable.SetActive(true);

            if (movableNightTable.TryGetComponent<PhysicalUnlock>(out PhysicalUnlock unlock))
            {
                unlock.Unlock();
            }
            else
            {
                Debug.LogError("PhysicalUnlock non trovato sul comodino!");
            }
        }
    }
}