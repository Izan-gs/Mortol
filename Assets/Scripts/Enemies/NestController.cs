using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestController : MonoBehaviour
{
    protected const int MAX_KILLED_COUNT = 3; // Class variable

    private int deadUnitsCount; // Individually count for each one
    private bool isAlive = true; // It can be death so nothing does
    private bool isActive = true; // It can be inactive if there are blockers in front of it
    private float timer; // Time running for spawns

    [SerializeField] private int spawnInterval; // Interval between spawns
    [SerializeField] private GameObject spawnUnitType; // Prefab

    //
    private void Awake()
    {
        spawnInterval = 3;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    // Calls Spawn in the chosen interval
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
        GameObject bumblebeeObj = Instantiate(spawnUnitType, transform.position, Quaternion.identity);
        Enemy enemy = bumblebeeObj.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.onDie.AddListener(IncrementDeadUnits);
        }
    }
    // The nest dies when player kills MAX_KILLED_COUNT units from nest.
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

// ASK
// Cast properly
// Disable correctly (Update)


// REMEMBER
// Herencia en C# - Segundo + David
// ScriptableObjects - Fran
// ENUM State Machine - Fran
// Ejemplos de segundo curso - David