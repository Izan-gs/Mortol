using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Connects the UI Settings Panel with the Settings Logic
public class SettingsUI : MonoBehaviour
{
    public void OnVolumeChanged(float value)
    {
        GameManager.Settings.SetMasterVolume(value);
    }
    public void OnBGMChanged(bool value)
    {
        GameManager.Settings.SetBGM(value);
    }
    public void OnSFXChanged(bool value)
    {
        GameManager.Settings.SetSFX(value);
    }
}
