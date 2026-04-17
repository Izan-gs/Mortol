using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseLogic : MonoBehaviour
{
    bool isPaused = false;
    public GameObject pauseObject;

    public void Pause()
    {
        Time.timeScale = 0.0f;
        pauseObject.SetActive(true);
    }
    public void UnPause()
    {
        Time.timeScale = 1.0f;
        pauseObject.SetActive(false);
    }

    // Function key

    public bool IsPaused()
    {
        return isPaused;
    }
}
