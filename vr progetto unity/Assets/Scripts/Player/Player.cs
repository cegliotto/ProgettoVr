using UnityEngine;
using System;

using static Player;

public class Player : MonoBehaviour {
    public static Player Instance; // Singleton

    // Struct contenente le informazioni per il cambio di scena
    public struct PlayerInfo {
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public float cameraXRotation;
        public float cameraYRotation;
    }

    public enum PlayerState { // Stato del player (verificare)
        Idle, // Default
        Movement, // In movimento
        Dialog, // Entra in dialogo -> voglio poter muovere la camera ma non far camminare
        Pause, // Non devo poter far muovere camera o camminare dallo script di playerController
    }
    public PlayerState playerState;

    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public PlayerInteract playerInteract;

    public Transform notebookHolder;

    private void Awake() {

        playerController = GetComponent<PlayerController>();
        playerInteract = GetComponent<PlayerInteract>();

        // Per gestione cambio scena 
        // Evitare di avere 2 istanze Singleton
        if (Instance == null) {
            Instance = this;
            return;
        }
        //DontDestroyOnLoad(gameObject);

        Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(PlayerState state) {
        playerState = state;
    }

    // Metodi usati per salvare le informazioni per il cambio di scena
    public PlayerInfo SaveInfo() {
        return new PlayerInfo {
            playerPosition = this.transform.position,
            playerRotation = this.transform.rotation,
            cameraXRotation = this.playerController.GetCameraXRotation(),
            cameraYRotation = this.playerController.GetCameraYRotation()
        };
    }

    public void LoadInfo(PlayerInfo info) {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = info.playerPosition;
        rb.rotation = info.playerRotation;

        playerController.SetCameraRotation(info.cameraXRotation, info.cameraYRotation);

        Debug.Log($"[RESTORE] {rb.position}");
    }
}
