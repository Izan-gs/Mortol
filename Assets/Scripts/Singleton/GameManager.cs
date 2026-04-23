using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        FindLivesText();
        UpdateLivesUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetForNewLevel();
    }

    private void ResetForNewLevel()
    {
        isFirstSpawnOfLevel = true;
        respawnQueued = false;

        currentPlayer = null;
        shipTransform = null;

        CacheShipTransform();
        FindLivesText();

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
        {
            Debug.Log("No lives left");
            return;
        }

        CameraController cam = GetCameraController();
        if (cam == null)
        {
            Debug.LogError("CameraController missing!");
            return;
        }

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
        {
            Debug.Log("No lives left");
            return;
        }

        CacheShipTransform();

        if (shipTransform == null)
        {
            Debug.LogError("Ship transform missing!");
            return;
        }

        // ONLY lose life on respawns, never on first spawn of level
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