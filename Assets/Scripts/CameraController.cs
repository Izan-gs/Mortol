using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float offsetX = 2f;

    [Header("Spawn Push")]
    [SerializeField] private float shipPushSpeed = 6f;

    [Header("Wall")]
    [SerializeField] private float wallOffset = 6f;
    [SerializeField] private float wallHeight = 10f;
    [SerializeField] private float wallThickness = 1f;

    [Header("Ship References")]
    [SerializeField] private GameObject spaceShip;

    private Transform player;
    private PlayerController currentPlayer;
    private bool stopFollowing;

    private BoxCollider2D wallCollider;
    private float wallX;

    private ShipCollisionDetector shipDetector;
    private bool waitingForSafeSpawn;
    private Action onSafeSpawnReady;

    private Vector3 basePosition;
    private float targetX;

    private float shakeTime;
    private float shakeIntensity;

    public BoxCollider2D GetWallCollider() => wallCollider;

    void Awake()
    {
        CreateWall();
        CacheShipReferences();

        basePosition = transform.position;
        targetX = transform.position.x;
    }

    void LateUpdate()
    {
        if (waitingForSafeSpawn)
        {
            HandleSafeSpawnMovement();
        }
        else
        {
            FollowPlayer();
        }

        ApplyCameraPosition();
        UpdateWall();

        if (!waitingForSafeSpawn)
        {
            CheckPlayerCrossing();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        player = newTarget;
        currentPlayer = player != null ? player.GetComponent<PlayerController>() : null;
    }

    public void RequestSafeSpawn(Action callback)
    {
        if (callback == null)
            return;

        CacheShipReferences();

        onSafeSpawnReady = callback;
        waitingForSafeSpawn = true;
    }

    private void HandleSafeSpawnMovement()
    {
        if (shipDetector == null)
            return;

        // ALWAYS force movement while blocked
        if (shipDetector.isCollidingWithPlatform)
        {
            targetX += shipPushSpeed * Time.deltaTime;

            // IMPORTANT: force instant camera follow (no smoothing delay blocking it)
            basePosition = new Vector3(
                targetX,
                basePosition.y,
                transform.position.z
            );

            return;
        }

        // ONLY when fully free
        FinishSafeSpawn();
    }

    private void FinishSafeSpawn()
    {
        waitingForSafeSpawn = false;

        Action callback = onSafeSpawnReady;
        onSafeSpawnReady = null;

        callback?.Invoke();
    }

    private void FollowPlayer()
    {
        if (player == null)
            return;

        if (currentPlayer != null)
            stopFollowing = currentPlayer.IsSticking();

        if (!stopFollowing)
        {
            float followX = player.position.x + offsetX;
            targetX = Mathf.Max(targetX, followX);
        }
    }

    private void ApplyCameraPosition()
    {
        basePosition = new Vector3(
            targetX,
            basePosition.y,
            transform.position.z
        );

        Vector3 shake = Vector3.zero;

        if (shakeTime > 0f)
        {
            shakeTime -= Time.deltaTime;
            shake = GetShakeOffset();
        }

        transform.position = basePosition + shake;
    }

    public void Shake(float duration, float intensity)
    {
        shakeTime = duration;
        shakeIntensity = intensity;
    }

    private Vector3 GetShakeOffset()
    {
        float strength = shakeIntensity * shakeTime;

        float x = UnityEngine.Random.Range(-1f, 1f) * strength;
        float y = UnityEngine.Random.Range(-1f, 1f) * strength;

        return new Vector3(x, y, 0f);
    }

    private void CacheShipReferences()
    {
        if (shipDetector != null)
            return;

        if (spaceShip == null)
        {
            Transform childShip = transform.Find("Space Ship");
            if (childShip != null)
            {
                spaceShip = childShip.gameObject;
            }
            else
            {
                GameObject foundShip = GameObject.Find("Space Ship");
                if (foundShip != null)
                    spaceShip = foundShip;
            }
        }

        if (spaceShip != null)
        {
            shipDetector = spaceShip.GetComponent<ShipCollisionDetector>();
        }
    }

    private void CreateWall()
    {
        GameObject wall = new GameObject("CameraWall");
        wall.transform.parent = transform;
        wall.layer = LayerMask.NameToLayer("CameraWall");

        wallCollider = wall.AddComponent<BoxCollider2D>();
        wallCollider.isTrigger = false;

        Rigidbody2D rb = wall.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;
    }

    private void UpdateWall()
    {
        if (wallCollider == null)
            return;

        wallX = transform.position.x - wallOffset;

        wallCollider.transform.position = new Vector2(wallX, transform.position.y);
        wallCollider.size = new Vector2(wallThickness, wallHeight);
    }

    private void CheckPlayerCrossing()
    {
        if (player == null || currentPlayer == null)
            return;

        bool playerIsPastWall = player.position.x < wallX;

        if (playerIsPastWall && currentPlayer.IsSticking())
        {
            StartCoroutine(KillAfterDelay());
        }
    }

    private IEnumerator KillAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        if (currentPlayer != null)
        {
            currentPlayer.GetComponent<PlayerController>().isInvulnerable = false;
            currentPlayer.Die();
        }
    }
}