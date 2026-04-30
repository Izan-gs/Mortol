using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Save to MongoDB (backend)
[Serializable]
public class PlayerSettings
{
    public bool BGM; // Background sound on/off
    public bool SFX; // SFX sound on/off

    public float masterVolume = 1f;
}
