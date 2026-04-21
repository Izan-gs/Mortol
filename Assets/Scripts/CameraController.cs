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

    public BoxCollider2D GetWallCollider() => wallCollider;

    void Awake()
    {
        CreateWall();
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

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(maxX, transform.position.y, transform.position.z),
            smoothSpeed * Time.deltaTime
        );

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