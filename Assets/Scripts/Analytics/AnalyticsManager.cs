using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    // It lives from StartLevel -> EndLevel
    private LevelAnalytics current;
    private bool isLevelActive = false;
    // It lives from Play -> ExitGame
    private GameSessionAnalytics session;
    private bool isSessioActive = false;

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
        GameEvents.OnLifeGain += HandleLifeGain;
        GameEvents.OnJump += HandleJump;
        GameEvents.OnShipMoved += HandleShipMoved;
        GameEvents.OnPlayerChangedDirection += HandleMovement;
    }

    private void OnDisable()
    {
        GameClock.OnTick -= HandleTick;
        GameEvents.OnPlayerStateChanged -= HandlePlayerStateChanged;
        GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        GameEvents.OnAbilityUsed -= HandleAbilityUsed;
        GameEvents.OnLifeLost -= HandleLifeLost;
        GameEvents.OnLifeGain -= HandleLifeGain;
        GameEvents.OnJump -= HandleJump;
        GameEvents.OnShipMoved -= HandleShipMoved;
        GameEvents.OnPlayerChangedDirection -= HandleMovement;
    }

    // |============================================================================================|
    #region Game Session Analytics
    public void StartGame(string sessionId)
    {
        session = GameSessionAnalyticsFactory.Create(sessionId);
        isSessioActive = true;
    }
    public void EndGame()
    {
        if (!isSessioActive) return;

        session.sessionEndTime = Time.time;
        isSessioActive = false;

        // Possible coroutine implementation
        string json = JsonUtility.ToJson(session, true);
        Debug.Log(json);
        
        // Enviar a backend (MongoDB)

        session = null; // Clears the instance
    }
    #endregion
    // |============================================================================================|
    #region Level Analytics
    // It creates a LevelAnalytics instance
    public void StartLevel(string levelId)
    {
        current = LevelAnalyticsFactory.Create(levelId);
        isLevelActive = true;
    }
    // It converts the LevelAnalytics instance into JSON send to backend and clears it
    public void EndLevel()
    {
        if (!isLevelActive) return;

        current.sessionEndTime = Time.time;
        isLevelActive = false;

        // En caso de querer convertir el nivel en JSON y guardarlo.
        //string json = JsonUtility.ToJson(current, true);
        //Debug.Log(json);

        // Adds the level to the Game Session Levels
        session.AddLevel(current);
        current = null; // Clears the instance
    }
    #endregion
    // |============================================================================================|
    // Checks a valid state of the analytics system:
    // 1- Exists the singleton manager "Instance"?
    // 2- Exists a current active level?
    // 3- Exists the level analytics object?
    private bool IsReady()
    {
        return Instance != null && isLevelActive && current != null;
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

        current.combat.RegisterKill(e.enemy);
    }
    private void HandleAbilityUsed(AbilityUsedEvent e)
    {
        if (!IsReady()) return;

        current.abilities.RegisterUse(e.ability);
    }
    private void HandleLifeLost(LifeLostEvent e)
    {
        if (!IsReady()) return;

        current.lives.AddLoss(e.loss);
    }
    private void HandleLifeGain(LifeGainEvent e)
    {
        if (!IsReady()) return;

        current.lives.AddGain(e.gain);
    }
    private void HandleJump(JumpEvent e)
    {
        if (!IsReady()) return;

        current.jumps.RegisterJump(e.height, e.distance);
    }

    public void HandleMovement(PlayerChangeDirectionEvent e)
    {
        current.movement.directionChanged();
    }
    public void HandleShipMoved(ShipMovedEvent e)
    {
        current.movement.shipMoved();
    }

    // GETTER
    public LevelAnalytics GetCurrentLevelData()
    {
        return current;
    }
}

// |============================================================================================|
// COMMENT EVERYTHING AND SAVE IT

// Mejorar el JUMP (Para mínimos y máximos) (Para esto, hay que hacer llamadas a la lógica al hacer el salto y al finalizarlo)
// TIEMPO (Añadir playerStates para que funcionen las métricas de tiempo)
// BLOQUES (Falta hacer los eventos específicos)
// ABILITY "COMBO_SAVER" (Falta hacer la lógica del combo saver)
// |============================================================================================|