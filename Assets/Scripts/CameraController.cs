using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public float smoothSpeed = 5f;
    public float offsetX = 2f;
    private bool waitingForSafeSpawn;
    private System.Action onSafeSpawnReady;

    [Header("Wall")]
    public float wallOffset = 6f;
    public float wallHeight = 10f;
    public float wallThickness = 1f;

    [Header("Ship Collision Push")]
    public float shipPushSpeed = 6f;
    public LayerMask platformMask;

    private Transform player;
    private float maxX;
    private bool stopFollowing;

    private BoxCollider2D wallCollider;
    private PlayerController currentPlayer;

    private float wallX;

    // Ship collision system
    public GameObject spaceShip;
    private ShipCollisionDetector shipDetector;
    private Coroutine shipPushRoutine;

    #region Camera Shake

    private float shakeTime;
    private float shakeIntensity;
    private Vector3 shakeOffset;
    private Vector3 basePosition;

    public void Shake(float duration, float intensity)
    {
        shakeTime = duration;
        shakeIntensity = intensity;
    }

    Vector3 GetShakeOffset()
    {
        float strength = shakeIntensity * (shakeTime);

        float x = Random.Range(-1f, 1f) * strength;
        float y = Random.Range(-1f, 1f) * strength;

        return new Vector3(x, y, 0f);
    }

    #endregion

    public BoxCollider2D GetWallCollider() => wallCollider;

    void Awake()
    {
        CreateWall();

        if (spaceShip != null)
        {
            shipDetector = spaceShip.GetComponent<ShipCollisionDetector>();
        }
    }

    void Start()
    {
        basePosition = transform.position;
    }

    public void SetTarget(Transform newTarget)
    {
        player = newTarget;

        if (player != null)
            currentPlayer = player.GetComponent<PlayerController>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        if (currentPlayer != null)
            stopFollowing = currentPlayer.IsSticking();

        if (!stopFollowing)
        {
            float targetX = player.position.x + offsetX;
            maxX = Mathf.Max(maxX, targetX);
        }

        basePosition = new Vector3(
            Mathf.Lerp(basePosition.x, maxX, smoothSpeed * Time.deltaTime),
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

        UpdateWall();
        CheckPlayerCrossing();

        if (waitingForSafeSpawn)
        {
            HandleSafeSpawnCheck();
        }

        PushCameraWhileShipBlocked();
    }

    public bool IsShipBlockedByPlatform()
    {
        if (spaceShip.GetComponent<Collider2D>() == null) return false;
        return spaceShip.GetComponent<Collider2D>().IsTouchingLayers(platformMask);
    }

    public void PushCameraWhileShipBlocked()
    {
        if (shipDetector == null) return;

        if (shipDetector.isCollidingWithPlatform)
        {
            maxX += shipPushSpeed * Time.deltaTime;
        }
    }

    public void PushCameraUntilShipIsFree()
    {
        if (spaceShip.GetComponent<Collider2D>() == null) return;

        if (shipPushRoutine != null)
            StopCoroutine(shipPushRoutine);

        shipPushRoutine = StartCoroutine(PushUntilFree());
    }

    IEnumerator PushUntilFree()
    {
        while (spaceShip.GetComponent<Collider2D>() != null && spaceShip.GetComponent<Collider2D>().IsTouchingLayers(platformMask))
        {
            maxX += shipPushSpeed * Time.deltaTime;
            yield return null;
        }

        shipPushRoutine = null;
    }

    void HandleSafeSpawnCheck()
    {
        if (spaceShip.GetComponent<Collider2D>() == null) return;

        // If still colliding, push camera right
        if (spaceShip.GetComponent<Collider2D>().IsTouchingLayers(platformMask))
        {
            maxX += shipPushSpeed * Time.deltaTime;
            return;
        }

        // When no longer colliding -> allow spawn
        waitingForSafeSpawn = false;

        onSafeSpawnReady?.Invoke();
        onSafeSpawnReady = null;
    }

    #region Ship collision push

    public void ResolveShipCollisionAfterSpawn()
    {
        if (spaceShip.GetComponent<Collider2D>() == null) return;

        if (shipPushRoutine != null)
            StopCoroutine(shipPushRoutine);

        shipPushRoutine = StartCoroutine(PushShipOutOfPlatform());
    }

    IEnumerator PushShipOutOfPlatform()
    {
        // Keep pushing camera right while ship is still colliding with platforms
        while (spaceShip.GetComponent<Collider2D>() != null && spaceShip.GetComponent<Collider2D>().IsTouchingLayers(platformMask))
        {
            maxX += shipPushSpeed * Time.deltaTime;
            yield return null;
        }

        shipPushRoutine = null;
    }

    #endregion

    #region Wall

    void CreateWall()
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

    void UpdateWall()
    {
        if (wallCollider == null) return;

        wallX = transform.position.x - wallOffset;

        wallCollider.transform.position = new Vector2(wallX, transform.position.y);
        wallCollider.size = new Vector2(wallThickness, wallHeight);
    }

    #endregion

    #region Manual Detection

    void CheckPlayerCrossing()
    {
        if (player == null || currentPlayer == null) return;

        bool playerIsPastWall = player.position.x < wallX;

        if (playerIsPastWall && currentPlayer.IsSticking())
        {
            StartCoroutine(KillAfterDelay());
        }
    }

    IEnumerator KillAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        if (currentPlayer != null)
        {
            currentPlayer.GetComponent<PlayerController>().isInvulnerable = false;
            currentPlayer.Die();
        }
    }

    public void RequestSafeSpawn(System.Action callback)
    {
        onSafeSpawnReady = callback;
        waitingForSafeSpawn = true;
    }

    #endregion
}