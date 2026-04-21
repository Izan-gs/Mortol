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
    private Animator animator; // Animator reference

    private void Awake()
    {
        spawnInterval = 3;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isActive || !isAlive) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            StartCoroutine(SpawnCoroutine());
            timer = 0;
        }
    }

    // Coroutine to play animation before spawning
    private IEnumerator SpawnCoroutine()
    {
        animator.SetTrigger("Spawn");

        yield return new WaitForSeconds(0.5f);

        Spawn();
    }

    // Spawns the selected enemy type.
    public void Spawn()
    {
        GameObject bumblebeeObj = Instantiate(spawnUnitType, transform.position, Quaternion.identity);

        BumblebeeController bee = bumblebeeObj.GetComponent<BumblebeeController>();
        if (bee != null)
        {
            bee.Initialize(transform.right, gameObject); // Spawn direction
        }
    }

    public void IncrementDeadUnits()
    {
        deadUnitsCount++;

        if (deadUnitsCount >= MAX_KILLED_COUNT)
        {
            isAlive = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check for player stone ritual in front of it then isAlive = false;
    }
}