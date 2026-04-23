using System.Collections;
using UnityEngine;
using TMPro;

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
    private GameObject currentPlayer;
    private bool firstSpawnDone;
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

    void Start()
    {
        UpdateLivesUI();
        SpawnPlayer();
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

        playerLives--;

        currentPlayer = Instantiate(playerPrefab, shipTransform.position, Quaternion.identity);

        PlayerController player = currentPlayer.GetComponent<PlayerController>();
        if (player != null)
        {
            player.StartParachute();
        }

        if (cam != null)
        {
            cam.SetTarget(currentPlayer.transform);
        }
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