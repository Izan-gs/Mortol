using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    public static GameClock Instance;

    public static event Action<float> OnTick;

    private bool isPaused;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (isPaused) return;

        OnTick?.Invoke(Time.deltaTime);
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }
}