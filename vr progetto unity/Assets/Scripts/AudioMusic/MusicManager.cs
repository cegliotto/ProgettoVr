using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("References")]
    [SerializeField] private AudioSource musicSource;

    [Header("Fade")]
    [SerializeField] private float defaultFadeDuration = 1.5f;

    [Header("Ducking (Dialogue)")]
    [Tooltip("Volume normale della musica quando non c'è dialogo.")]
    [SerializeField] private float normalMusicVolume = 1f;

    [Tooltip("Volume della musica durante il dialogo (duck).")]
    [Range(0f, 1f)]
    [SerializeField] private float duckedMusicVolume = 0.25f;

    private Coroutine routine;

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

        // Impostazione sicurezza
        if (musicSource != null)
        {
            musicSource.spatialBlend = 0f; // 2D
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }
    }

    // PLAY

    public void PlayMusic(AudioClip newClip, float targetVolume = 1f, float fadeDuration = -1f)
    {
        if (newClip == null || musicSource == null) return;
        if (musicSource.clip == newClip && musicSource.isPlaying) return;

        if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;

        // aggiorna volume "normale" di riferimento
        normalMusicVolume = Mathf.Clamp01(targetVolume);

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeToClip(newClip, normalMusicVolume, fadeDuration));
    }

    public void PlayMusicImmediate(AudioClip newClip, float volume = 1f)
    {
        if (newClip == null || musicSource == null) return;

        normalMusicVolume = Mathf.Clamp01(volume);

        if (routine != null) StopCoroutine(routine);

        musicSource.volume = normalMusicVolume;
        musicSource.clip = newClip;
        musicSource.Play();
    }

    //STOP

    public void FadeOutAndStop(float fadeDuration = -1f)
    {
        if (musicSource == null) return;
        if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeOutStopRoutine(fadeDuration));
    }

    public void StopImmediate()
    {
        if (musicSource == null) return;

        if (routine != null) StopCoroutine(routine);
        musicSource.Stop();
        musicSource.clip = null;
    }

    // Abbassa la musica (duck) con un fade. Chiamalo quando inizia il dialogo.
    public void DuckForDialogue(float fadeDuration = 0.25f)
    {
        if (musicSource == null) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeVolume(musicSource.volume, duckedMusicVolume, fadeDuration));
    }

    // Riporta la musica al volume normale con un fade. Chiamalo quando finisce il dialogo.
    public void UnduckAfterDialogue(float fadeDuration = 0.35f)
    {
        if (musicSource == null) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeVolume(musicSource.volume, normalMusicVolume, fadeDuration));
    }

    // Se vuoi impostare al volo quanto deve essere basso il duck (es. dialoghi più o meno importanti).
    public void SetDuckedVolume(float v) => duckedMusicVolume = Mathf.Clamp01(v);


    private IEnumerator FadeToClip(AudioClip newClip, float targetVolume, float duration)
    {
        // Fade out se sta suonando
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

        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
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

    private IEnumerator FadeVolume(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            musicSource.volume = to;
            yield break;
        }

        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        musicSource.volume = to;
    }
}
