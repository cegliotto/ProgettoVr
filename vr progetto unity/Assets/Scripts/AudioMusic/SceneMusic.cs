using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public enum OnSceneStartMode
    {
        PlayThisMusic,   // Fade-out eventuale + fade-in di questa clip
        Silence,         // Fade-out e stop (scena senza musica)
        DoNothing        // Non tocca la musica (continua quella precedente)
    }

    [Header("On Scene Start")]
    [SerializeField] private OnSceneStartMode mode = OnSceneStartMode.PlayThisMusic;

    [SerializeField] private AudioClip sceneMusic;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float fadeDuration = 1.5f;

    private void Start()
    {
        if (MusicManager.Instance == null) return;

        switch (mode)
        {
            case OnSceneStartMode.PlayThisMusic:
                if (sceneMusic != null)
                    MusicManager.Instance.PlayMusic(sceneMusic, volume, fadeDuration);
                break;

            case OnSceneStartMode.Silence:
                MusicManager.Instance.FadeOutAndStop(fadeDuration);
                break;

            case OnSceneStartMode.DoNothing:
                // lascia com'è
                break;
        }
    }
}
