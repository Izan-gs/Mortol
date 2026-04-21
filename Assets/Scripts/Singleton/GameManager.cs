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

        if (shipTransform == null)
        {
            shipTransform = GameObject.Find("Space Ship")?.transform;

            if (shipTransform == null)
            {
                Debug.LogError("Ship transform missing!");
                return;
            }
        }

        playerLives--;

        currentPlayer = Instantiate(playerPrefab, shipTransform.position, Quaternion.identity);

        PlayerController player = currentPlayer.GetComponent<PlayerController>();
        player.StartParachute();

        // Register player to camera
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