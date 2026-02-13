using UnityEngine;

public class IntroController : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private GameObject startButton;

    private void Start()
    {
        //if (startButton != null)
        //startButton.SetActive(true);

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetContext(CursorContext.UI);
    }

    public void StartGame()
    {
        levelLoader.LoadNextScene("trainStation");
    }
}
