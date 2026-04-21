using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Lives")]
    public int playerLives = 5;

    [Header("Spawn")]
    public Transform shipTransform;
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
    }

    void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if (playerLives <= 0)
        {
            Debug.Log("No lives left"); // Implementar aquí el activar la UI de muerte
            return;
        }

        playerLives--;

        currentPlayer = Instantiate(playerPrefab, shipTransform.position, Quaternion.identity);
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