using UnityEngine;

public class NecklaceVisibility : MonoBehaviour
{
    void Start()
    {
        // Verifichiamo se il PuzzleManager esiste nella scena
        if (PuzzleManager.Instance != null)
        {
            // Controlliamo se il puzzle delle viti è già stato risolto
            bool isSolved = PuzzleManager.Instance.isPuzzleSolved(PuzzleType.PuzzleScrews);

            // Attiva o disattiva la collana in base allo stato del puzzle
            gameObject.SetActive(isSolved);
        }
        else
        {
            // Se il manager non è presente (es. test rapido), nascondi l'oggetto per sicurezza
            gameObject.SetActive(false);
        }
    }
}