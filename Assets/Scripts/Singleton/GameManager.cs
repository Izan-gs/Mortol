using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Lives")]
    public int playerLives = 20;

    [Header("Spawn")]
    private Transform shipTransform;
    public GameObject playerPrefab;

    private GameObject currentPlayer;
    private bool firstSpawnDone = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Avoid deleting when changing scene
        }
        else
        {
            Destroy(gameObject);
        }

        // Find Space Ship transform (safe version)
        shipTransform = GameObject.Find("Space Ship")?.transform;
    }

    void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if (playerLives <= 0)
        {
            Debug.Log("No lives left");
            return;
        }

        CameraController cam = Camera.main.GetComponent<CameraController>();
        if (cam == null)
        {
            Debug.LogError("Camera missing!");
            return;
        }

        playerLives--;

        // FIRST SPAWN = instant
        if (!firstSpawnDone)
        {
            firstSpawnDone = true;
            DoSpawnPlayer();
            return;
        }

        StartCoroutine(SafeSpawnRoutine(cam));
    }

    IEnumerator SafeSpawnRoutine(CameraController cam)
    {
        ShipCollisionDetector detector = cam.GetComponentInChildren<ShipCollisionDetector>();

        if (detector == null)
        {
            Debug.LogError("Ship detector missing!");
            yield break;
        }

        while (detector.isCollidingWithPlatform)
        {
            yield return null;
        }

        DoSpawnPlayer();
    }

    void DoSpawnPlayer()
    {
        if (shipTransform == null)
        {
            shipTransform = GameObject.Find("Space Ship")?.transform;

            if (shipTransform == null)
            {
                Debug.LogError("Ship transform missing!");
                return;
            }
        }

        currentPlayer = Instantiate(playerPrefab, shipTransform.position, Quaternion.identity);

        PlayerController player = currentPlayer.GetComponent<PlayerController>();
        player.StartParachute();

        CameraController cam = Camera.main.GetComponent<CameraController>();
        if (cam != null)
        {
            cam.SetTarget(currentPlayer.transform);
        }
    }

    public void PlayerDied()
    {
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SpawnPlayer();
    }
}