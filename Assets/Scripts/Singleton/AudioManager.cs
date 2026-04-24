using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer mixer;

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

    // GLOBAL VOLUME (MASTER)
    public void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);

        float db = Mathf.Lerp(-80f, 0f, value);
        mixer.SetFloat("MasterVolume", db);
    }
}