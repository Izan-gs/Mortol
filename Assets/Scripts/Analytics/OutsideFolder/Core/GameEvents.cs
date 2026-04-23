using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INTERNAL DISPATCHER
public static class GameEvents
{
    public static event Action<EnemyKilledEvent> OnEnemyKilled;

    public static void Emit(EnemyKilledEvent e)
    {
        OnEnemyKilled?.Invoke(e);
    }


    public static event Action<AbilityUsedEvent> OnAbilityUsed;

    public static void Emit(AbilityUsedEvent e)
    {
        OnAbilityUsed?.Invoke(e);
    }

    public static event Action<LifeLostEvent> OnLifeLost;

    public static void Emit(LifeLostEvent e)
    {
        OnLifeLost?.Invoke(e);
    }
    public static event Action<JumpEvent> OnJump;

    public static void Emit(JumpEvent e)
    {
        OnJump?.Invoke(e);
    }
    public static event Action<PlayerStateChangedEvent> OnPlayerStateChanged;

    public static void Emit(PlayerStateChangedEvent e)
    {
        OnPlayerStateChanged?.Invoke(e);
    }
}
