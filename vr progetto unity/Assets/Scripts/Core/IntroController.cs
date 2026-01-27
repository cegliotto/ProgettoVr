using UnityEngine;

public class IntroController : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private GameObject startButton;

    private void Start()
    {
        //if (startButton != null)
            //startButton.SetActive(true);
    }

    public void StartGame()
    {
        levelLoader.LoadNextScene("trainStation");
    }
}
