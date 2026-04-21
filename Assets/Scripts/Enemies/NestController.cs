using System.Collections;
using UnityEngine;

public class NestController : MonoBehaviour
{
    private const int MAX_KILLED_COUNT = 3;
    private const float SPAWN_ANIMATION_DELAY = 0.5f;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private GameObject spawnUnitType;

    [Header("Detection")]
    [SerializeField] private float frontCheckDistance = 1f;
    [SerializeField] private LayerMask platformLayer;

    private int deadUnitsCount;
    private bool isAlive = true;
    private bool isActive = true;

    private float timer;
    private Animator animator;

    private readonly RaycastHit2D[] hitBuffer = new RaycastHit2D[4];

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isActive || !isAlive)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer -= spawnInterval;
            TrySpawn();
        }
    }

    private void TrySpawn()
    {
        if (IsBlockedInFront())
            return;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        animator.SetTrigger("Spawn");

        yield return new WaitForSeconds(SPAWN_ANIMATION_DELAY);

        if (!isAlive || IsBlockedInFront())
            yield break;

        Spawn();
    }

    private void Spawn()
    {
        if (spawnUnitType == null)
            return;

        GameObject obj = Instantiate(spawnUnitType, transform.position, Quaternion.identity);

        if (obj.TryGetComponent(out BumblebeeController bee))
        {
            bee.Initialize(transform.right, gameObject);
        }
    }

    private bool IsBlockedInFront()
    {
        Vector2 origin = transform.position + transform.right * 0.25f;
        Vector2 direction = transform.right;

        int hitCount = Physics2D.RaycastNonAlloc(
            origin,
            direction,
            hitBuffer,
            frontCheckDistance,
            platformLayer
        );

        Debug.DrawRay(origin, direction * frontCheckDistance, Color.red);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D col = hitBuffer[i].collider;

            if (col == null || col.gameObject == gameObject)
                continue;

            return true;
        }

        return false;
    }

    public void IncrementDeadUnits()
    {
        deadUnitsCount++;

        if (deadUnitsCount >= MAX_KILLED_COUNT)
        {
            isAlive = false;
        }
    }
}