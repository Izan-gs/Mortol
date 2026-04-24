using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer mixer;

    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Sounds")]
    public AudioClip jumpSound;
    public AudioClip turnToStoneSound;
    public AudioClip stickSound;
    public AudioClip explosionSound;
    public AudioClip dieSound;
    public AudioClip nestSpawnSound;
    public AudioClip destroyBrickSound;
    public AudioClip activatorPlatformSound;
    public AudioClip lifeBlockSound;

    void Awake()
    {
        instance = this;
    }

    // BGM TOGGLE
    public void SetBGM(bool enabled)
    {
        mixer.SetFloat("BGMVolume", enabled ? 0f : -80f);
    }

    // SFX TOGGLE
    public void SetSFX(bool enabled)
    {
        mixer.SetFloat("SFXVolume", enabled ? 0f : -80f);
    }

    // MASTER VOLUME
    public void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);
        float db = Mathf.Lerp(-80f, 0f, value);
        mixer.SetFloat("MasterVolume", db);
    }

    // ALWAYS SFX GROUP
    public AudioSource PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
            return null;

        GameObject tempGO = new GameObject("TempAudio");
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = sfxGroup;
        audioSource.volume = volume;

        audioSource.Play();

        Destroy(tempGO, clip.length / audioSource.pitch);

        return audioSource;
    }
}