using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettingsService
{
    // Chages occurred detector
    private bool isDirty;

    public PlayerSettings Current { get; private set; }

    // If there is saved data, I use it, if not, I create it.
    public void Load(PlayerSettings data)
    {
        Current = data ?? new PlayerSettings();
    }
    public void SetMasterVolume(float value)
    {
        Current.masterVolume = value;
        isDirty = true;
    }

    public void SetBGM(bool enabled)
    {
        Current.BGM = enabled;
        isDirty = true;
    }

    public void SetSFX(bool enabled)
    {
        Current.SFX = enabled;
        isDirty = true;
    }
    // Checks if there are any changes, if so, it resets to false, and returns the correct state
    public bool ConsumeDirtyFlag()
    {
        if (!isDirty) return false;

        isDirty = false;
        return true;
    }
}