using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioMixerSnapshot fullVolume;
    [SerializeField] AudioMixerSnapshot noVolume;
    [SerializeField] float transitionSpeed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fullVolume = mixer.FindSnapshot("fullVolume");
        noVolume = mixer.FindSnapshot("noVolume");
        noVolume.TransitionTo(0.01f); //per essere sicuri
        fullVolume.TransitionTo(transitionSpeed);
    }
}
