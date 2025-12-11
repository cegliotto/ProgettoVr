using UnityEngine;

public class Player : MonoBehaviour {
    public static Player Instance; // Singleton

    public enum PlayerState { // Stato del player (verificare)
        Idle, // Default
        Movement, // In movimento
        Dialog, // Entra in dialogo -> voglio poter muovere la camera ma non far camminare
        Puzzle, // Non devo poter far muovere camera o camminare dallo script di playerController
    }
    public PlayerState playerState;

    [HideInInspector] public PlayerController playerController;

    private void Awake() {

        playerController = GetComponent<PlayerController>();

        // Per gestione cambio scena 
        // Evitare di avere 2 istanze Singleton
        if (Instance == null) {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }
}
