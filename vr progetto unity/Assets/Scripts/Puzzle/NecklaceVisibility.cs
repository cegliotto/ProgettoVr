using UnityEngine;

public class NecklaceVisibility : MonoBehaviour
{
    void Start()
    {
        // si verifica se il PuzzleManager esiste nella scena
        if (PuzzleManager.Instance != null)
        {
            // si conrolla se il puzzle delle viti è già stato risolto
            bool isSolved = PuzzleManager.Instance.isPuzzleSolved(PuzzleType.PuzzleScrews);

            // si attiva o disattiva la collana in base allo stato del puzzle
            gameObject.SetActive(isSolved);
        }
        else
        {
            // Se il manager non è presente si nasconde l'oggetto per sicurezza
            gameObject.SetActive(false);
        }
    }
}