using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public StatsPanelUI statsPanel;

    [Header("Lives")]
    public int playerLives = 20;

    [Header("Spawn")]
    [SerializeField] private GameObject playerPrefab;

    [Header("UI")]
    [SerializeField] private TMP_Text livesText;

    private Transform shipTransform;
    public GameObject currentPlayer;
    private bool firstSpawnDone;
    private bool isFirstSpawnOfLevel = true;
    private bool respawnQueued;

    // Pause
    private GameObject pauseText;
    private bool isPaused;
    // Settings
    public static PlayerSettingsService Settings;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CacheShipTransform();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // Settings init
        Settings = new PlayerSettingsService();
        PlayerSettings loaded = LoadSettings(); // Loading Player Settings
        Settings.Load(loaded);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        // Settings check for changes
        if (Settings.ConsumeDirtyFlag())
        {
            SaveSettings();
        }
    }
    // =====================================================================================================| SETTINGS
    public void SaveSettings()
    {
        var json = JsonUtility.ToJson(Settings.Current, true);
        Debug.Log(json);

        // TODO: guardar en archivo o Mongo
    }
    private PlayerSettings LoadSettings()
    {
        // De momento no tienes persistencia -> devolvemos null
        return null;
    }
    // =====================================================================================================|
    private void TogglePause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        FindPauseText();

        if (pauseText != null)
        {
            TMP_Text text = pauseText.GetComponent<TMP_Text>();

            if (text != null)
                text.alpha = isPaused ? 1f : 0f;
        }
    }

    private void FindPauseText()
    {
        if (pauseText == null)
        {
            pauseText = GameObject.Find("Pause Text");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // =====================================================================================================| ANALYTICS FLOW
    void Start()
    {
        FindLivesText();
        UpdateLivesUI();
        // Analytics Game Session starts
        AnalyticsManager.Instance.StartGame(System.Guid.NewGuid().ToString());
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetForNewLevel();
        // Ends the level analytics
        AnalyticsManager.Instance.EndLevel();
        // Starts the new level analytics
        AnalyticsManager.Instance.StartLevel(scene.name);
        // Panel stats showing
        if (statsPanel != null)
        {
            statsPanel.Show(AnalyticsManager.Instance.GetCurrentLevelData());
        }
    }
    // END GAME
    private void OnApplicationQuit()
    {
        // Save analytics
        AnalyticsManager.Instance.EndLevel();
        AnalyticsManager.Instance.EndGame();
        // Save settings
        SaveSettings();
    }
    // =====================================================================================================|
    private void ResetForNewLevel()
    {
        isFirstSpawnOfLevel = true;
        respawnQueued = false;

        currentPlayer = null;
        shipTransform = null;

        CacheShipTransform();
        FindLivesText();

        pauseText = null;
        isPaused = false;
        Time.timeScale = 1f;

        UpdateLivesUI();
        SpawnPlayer();
    }

    private void FindLivesText()
    {
        GameObject obj = GameObject.Find("Lifes Text");

        if (obj != null)
            livesText = obj.GetComponent<TMP_Text>();
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = playerLives.ToString();
    }

    public void SpawnPlayer()
    {
        UpdateLivesUI();

        if (playerLives <= 0)
            return;

        CameraController cam = GetCameraController();
        if (cam == null)
            return;

        if (!firstSpawnDone)
        {
            firstSpawnDone = true;
            SpawnPlayerNow(cam);
            return;
        }

        cam.RequestSafeSpawn(() => SpawnPlayerNow(cam));
    }

    private void SpawnPlayerNow(CameraController cam)
    {
        if (playerLives <= 0)
            return;

        CacheShipTransform();

        if (shipTransform == null)
            return;

        if (!isFirstSpawnOfLevel)
            playerLives--;

        isFirstSpawnOfLevel = false;

        currentPlayer = Instantiate(playerPrefab, shipTransform.position, Quaternion.identity);

        PlayerController player = currentPlayer.GetComponent<PlayerController>();
        if (player != null)
        {
            player.StartParachute();

            Exit exit = FindAnyObjectByType<Exit>();
            if (exit != null) exit.player = player;
        }

        if (cam != null)
        {
            cam.SetTarget(currentPlayer.transform);
        }

        UpdateLivesUI();
    }

    public void PlayerDied()
    {
        if (respawnQueued)
            return;

        respawnQueued = true;
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(1f);
        respawnQueued = false;
        SpawnPlayer();
    }

    private void CacheShipTransform()
    {
        if (shipTransform != null)
            return;

        GameObject ship = GameObject.Find("Space Ship");
        if (ship != null)
            shipTransform = ship.transform;
    }

    private CameraController GetCameraController()
    {
        Camera mainCamera = Camera.main;
        return mainCamera != null ? mainCamera.GetComponent<CameraController>() : null;
    }
}