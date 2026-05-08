using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Referencia al gestor de preferencias
    public PreferencesManager preferencesManager;

    // Panel de estadísticas
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

    // UI de pausa
    private GameObject pauseText;
    private bool isPaused;

    // Servicio de ajustes de jugador
    public static PlayerSettingsService Settings;

    private void Awake()
    {
        // Singleton
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

        // Inicialización de settings
        Settings = new PlayerSettingsService();
        PlayerSettings loaded = LoadSettings();
        Settings.Load(loaded);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // Si cambian los ajustes, se guardan
        if (Settings.ConsumeDirtyFlag())
        {
            SaveSettings();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Main Menu");
        }
    }

    // Ajustes

    // Guarda los ajustes del jugador, de momento solo se muestran por consola
    public void SaveSettings()
    {
        var json = JsonUtility.ToJson(Settings.Current, true);
        Debug.Log(json);
    }

    // Carga los ajustes del jugador, todavía no hay persistencia para este bloque
    private PlayerSettings LoadSettings()
    {
        return null;
    }

    // Guarda las vidas restantes del nivel actual, en el archivo local y luego las sube a la base de datos
    public void SaveLevelLives()
    {
        PreferencesData data = preferencesManager.Load();

        string scene = SceneManager.GetActiveScene().name;

        if (scene.Contains("Level 1"))
            data.level1Lives = playerLives;
        else if (scene.Contains("Level 2"))
            data.level2Lives = playerLives;

        preferencesManager.Save(data);
        preferencesManager.ImportDatabase();
    }

    // Pausa
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

    // Analytics
    private void Start()
    {
        FindLivesText();
        UpdateLivesUI();

        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.StartGame(System.Guid.NewGuid().ToString());
        }

        if (preferencesManager == null)
        {
            preferencesManager = FindAnyObjectByType<PreferencesManager>();

            if (preferencesManager == null)
            {
                Debug.LogWarning("PreferencesManager not found.");
                return;
            }
        }

        PreferencesData data = preferencesManager.Load();

        string scene = SceneManager.GetActiveScene().name;

        if (scene == "Level 1")
            playerLives = data.level1Lives > 0 ? data.level1Lives : playerLives;
        else if (scene == "Level 2")
            playerLives = data.level2Lives > 0 ? data.level2Lives : playerLives;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetForNewLevel();

        AnalyticsManager.Instance.EndLevel();
        AnalyticsManager.Instance.StartLevel(scene.name);

        if (statsPanel != null)
        {
            statsPanel.Show(AnalyticsManager.Instance.GetCurrentLevelData());
        }
    }

    // Al cerrar el juego se guarda todo lo que quede pendiente
    private void OnApplicationQuit()
    {
        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.EndLevel();
            AnalyticsManager.Instance.EndGame();
        }

        SaveSettings();

        if (preferencesManager != null)
        {
            preferencesManager.ImportDatabase();
        }
    }

    // Niveles / Respawn
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

    public void UpdateLivesUI()
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

        currentPlayer = Instantiate(
                playerPrefab,
                shipTransform.position,
                Quaternion.identity
        );

        PlayerController player = currentPlayer.GetComponent<PlayerController>();
        if (player != null)
        {
            player.StartParachute();

            Exit exit = FindAnyObjectByType<Exit>();
            if (exit != null)
                exit.player = player;
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

        if (playerLives <= 0)
        {
            yield return new WaitForSeconds(2f);

            Time.timeScale = 1f;
            SceneManager.LoadScene("Main Menu");

            yield break;
        }

        SpawnPlayer();
    }

    // Utilidades
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