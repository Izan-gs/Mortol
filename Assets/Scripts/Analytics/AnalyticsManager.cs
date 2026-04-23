using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    private LevelAnalytics current;
    private bool isLevelActive = false;

    // |============================================================================================|

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameClock.OnTick += HandleTick;
        GameEvents.OnPlayerStateChanged += HandlePlayerStateChanged;
        GameEvents.OnEnemyKilled += HandleEnemyKilled;
        GameEvents.OnAbilityUsed += HandleAbilityUsed;
        GameEvents.OnLifeLost += HandleLifeLost;
        GameEvents.OnJump += HandleJump;
    }

    private void OnDisable()
    {
        GameClock.OnTick -= HandleTick;
        GameEvents.OnPlayerStateChanged -= HandlePlayerStateChanged;
        GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        GameEvents.OnAbilityUsed -= HandleAbilityUsed;
        GameEvents.OnLifeLost -= HandleLifeLost;
        GameEvents.OnJump -= HandleJump;
    }

    // |============================================================================================|

    #region Basic methods
    public void StartLevel(string levelId)
    {
        current = LevelAnalyticsFactory.Create(levelId);
        isLevelActive = true;
    }

    public void EndLevel()
    {
        if (!isLevelActive) return;

        current.sessionEndTime = Time.time;
        isLevelActive = false;

        string json = JsonUtility.ToJson(current, true);
        Debug.Log(json);

        // Enviar a backend (MongoDB)
    }

    // Checks a valid state of the analytics system:
    // 1- Exists the singleton manager "Instance"?
    // 2- Exists a current active level?
    // 3- Exists the level analytics object?
    private bool IsReady()
    {
        return Instance != null && isLevelActive && current != null;
    }
    #endregion

    // ?¿?¿
    public void RegisterDirectionChange()
    {
        current.movement.directionChanges++;
    }

    // |==========================================================================================================|
    // |===============================================================================|
    //                                  API - Handler Register
    // |===============================================================================|
    // NEW SYSTEM

    // Handles the TIME
    private void HandleTick(float dt)
    {
        if (!IsReady()) return;

        current.time.Tick(dt);
    }
    private void HandlePlayerStateChanged(PlayerStateChangedEvent e)
    {
        if (!IsReady()) return;

        current.time.SetState(e.state);
    }
    private void HandleEnemyKilled(EnemyKilledEvent e)
    {
        if (!IsReady()) return;

        current.combat.enemiesKilled.RegisterKill(e.enemy);
    }
    private void HandleAbilityUsed(AbilityUsedEvent e)
    {
        if (!IsReady()) return;

        current.abilities.RegisterUse(e.ability);
    }
    private void HandleLifeLost(LifeLostEvent e)
    {
        if (!IsReady()) return;

        current.lives.lost.AddLoss(e.source);
    }
    private void HandleJump(JumpEvent e)
    {
        if (!IsReady()) return;

        current.jumps.RegisterJump(e.height, e.distance);
    }
}

// |======|
// TO-DO
// |======|
// COMMENT EVERYTHING AND SAVE IT
// CHECK THAT ALL IN THE DOCS IS IMPLEMENTED
// CHECK THAT ALL THE SYSTEM LOGIC IS IMPLEMENTED WELL
// ABILITY "SPECIFIED IN DOCS"