using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("References")]
    [SerializeField] private AudioSource musicSource;

    [Header("Fade")]
    [SerializeField] private float defaultFadeDuration = 1.5f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
    }

    
    /// Play con fade-out della clip corrente (se presente) + fade-in della nuova.
    
    public void PlayMusic(AudioClip newClip, float targetVolume = 1f, float fadeDuration = -1f)
    {
        if (newClip == null) return;
        if (musicSource.clip == newClip && musicSource.isPlaying) return;

        if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeToClip(newClip, targetVolume, fadeDuration));
    }

    /// Play immediato (senza fade-in).
    
    public void PlayMusicImmediate(AudioClip newClip, float volume = 1f)
    {
        if (newClip == null) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        musicSource.volume = volume;
        musicSource.clip = newClip;
        musicSource.Play();
    }
    
    /// Fade-out e stop (scena senza musica / inizio scena silenziosa).
    public void FadeOutAndStop(float fadeDuration = -1f)
    {
        if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOutStopRoutine(fadeDuration));
    }

    
    /// Stop immediato (senza fade).
    public void StopImmediate()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        musicSource.Stop();
        musicSource.clip = null;
    }

    private IEnumerator FadeToClip(AudioClip newClip, float targetVolume, float duration)
    {
        // Se c'è musica in corso, fade-out
        if (musicSource.isPlaying && musicSource.volume > 0f)
        {
            float startVol = musicSource.volume;
            for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
                yield return null;
            }
            musicSource.volume = 0f;
        }

        // Cambia clip e play
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade-in
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, targetVolume, t / duration);
            yield return null;
        }
        musicSource.volume = targetVolume;
    }

    private IEnumerator FadeOutStopRoutine(float duration)
    {
        if (!musicSource.isPlaying)
        {
            musicSource.clip = null;
            yield break;
        }

        float startVol = musicSource.volume;

        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
        musicSource.clip = null;
    }
}
