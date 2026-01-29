using UnityEngine;
using UnityEngine.SceneManagement;

public class Persistency : MonoBehaviour {
    public static Persistency Instance;

    [SerializeField] private string sceneToDestroyIn = "OutroScene";

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == sceneToDestroyIn) {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
