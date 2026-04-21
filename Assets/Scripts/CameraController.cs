using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public float smoothSpeed = 5f;
    public float offsetX = 2f;

    [Header("Wall")]
    public float wallOffset = 6f;
    public float wallHeight = 10f;
    public float wallThickness = 1f;

    private Transform player;
    private float maxX;
    private bool playerIsPastWall;
    private bool stopFollowing;

    private BoxCollider2D wallCollider;
    private PlayerController currentPlayer;

    private float wallX;

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

        // Store clean base position (NO shake contamination)
        basePosition = new Vector3(
            Mathf.Lerp(basePosition.x, maxX, smoothSpeed * Time.deltaTime),
            basePosition.y,
            transform.position.z
        );

        // Shake
        Vector3 shake = Vector3.zero;

        if (shakeTime > 0f)
        {
            shakeTime -= Time.deltaTime;
            shake = GetShakeOffset();
        }

        transform.position = basePosition + shake;

        UpdateWall();
        CheckPlayerCrossing();
    }

    #region Wall

    void CreateWall()
    {
        GameObject wall = new GameObject("CameraWall");
        wall.transform.parent = transform;

        // Assign CameraWall layer
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

        playerIsPastWall = player.position.x < wallX;

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

    #endregion
}