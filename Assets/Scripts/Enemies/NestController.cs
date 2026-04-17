using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestController : MonoBehaviour
{
    // Variable de clase constante
    protected const int MAX_KILLED_COUNT = 3;

    //
    private int deadUnitsCount;
    private bool isAlive;
    private bool isActive;
    private float timer;

    //
    [SerializeField]
    private int spawnInterval;
    [SerializeField]
    private Enemy spawnUnitType; // Prefab

    //
    private void Awake()
    {
        isAlive = true;
        isActive = true;
        spawnInterval = 3;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    // Calls Spawn in the interval selected
    void Update()
    {
        if (!isActive || !isAlive) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            Spawn();
            timer = 0;
        }
    }
    // Spawns the selected enemy type.
    public void Spawn()
    {
        Instantiate(spawnUnitType, transform.position, Quaternion.identity);
    }
    // The nest dies when player kills MAX_KILLED_COUNT units.
    public void IncrementDeadUnits()
    {
        deadUnitsCount++;

        if (deadUnitsCount >= MAX_KILLED_COUNT)
        {
            isAlive = false;
        }
    }
    // 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check for player stone ritual in front of it then isAlive = false;
    }
}

// Nest reference
    // Subscribing events
// Cast properly
// Disable correctly (Update)

// Constructor ? Init variables