using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
    public static LevelLoader Instance;
    [SerializeField] private Animator anim;
    [SerializeField] private float transitionTime = 1f;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void LoadNextScene(string sceneName) {
        InputManager.Instance.SetInputEnabled(false); // disattivo input di mouse e movimento. Li riattivo in Start() del Player
        StartCoroutine(LoadLevel(sceneName));
    }

    private IEnumerator LoadLevel(string sceneName) {
        anim.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
