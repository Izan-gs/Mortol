using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Components

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private CameraController cameraController;

    #endregion

    #region Colliders

    [Header("Colliders")]
    public CapsuleCollider2D capsuleCollider;
    public BoxCollider2D boxCollider;
    public BoxCollider2D boxCollider2;
    public BoxCollider2D boxColliderDeadSpikes;

    #endregion

    #region FX

    [Header("FX")]
    public GameObject explosionParticle;
    [SerializeField] private GameObject deadParticle;

    #endregion

    #region Movement

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput;
    private float lockedY;
    private bool lockYPosition;

    #endregion

    #region Jump

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    private bool isJumping;

    #endregion

    #region Jump Assist

    [Header("Jump Assist")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    private float coyoteCounter;
    private float bufferCounter;

    #endregion

    #region Ground

    [Header("Ground")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private bool grounded;

    #endregion

    #region State

    private bool isStone;
    private bool wasFallingStone;
    private bool isStuck;
    private bool isSticking;
    private bool controlsLocked;

    public bool canDamageEnemies;
    public bool isInvulnerable;

    #endregion

    #region Parachute

    [Header("Parachute")]
    [SerializeField] private float parachuteFallSpeed = 0.7f;
    [SerializeField] private float blinkInterval = 0.1f;

    private bool isParachuting;
    private Coroutine blinkCoroutine;

    #endregion

    #region Animator Hashes

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int StoneHash = Animator.StringToHash("Stone");
    private static readonly int StuckHash = Animator.StringToHash("IsStuck");
    private static readonly int StickHash = Animator.StringToHash("IsSticking");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int TorpedoHash = Animator.StringToHash("Torpedo");
    private static readonly int CrashHash = Animator.StringToHash("Crash");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int ParachuteHash = Animator.StringToHash("Parachute");

    #endregion

    #region Layers

    private int playerLayer;
    private int enemyLayer;
    private int platformLayer;
    private int stickingLayer;

    #endregion

    private float animSpeed;

    public bool IsSticking() => isSticking;
    public bool IsStoneFalling() => isStone && wasFallingStone;

    private Coroutine stickTimeoutCoroutine;
    private bool stuckToPlatform;

    #region Unity

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraController = FindAnyObjectByType<CameraController>();

        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        platformLayer = LayerMask.NameToLayer("Platform");
        stickingLayer = LayerMask.NameToLayer("PlayerSticking");

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    }

    private void Update()
    {
        CheckGround();
        HandleJumpAssist();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (isStuck) return;

        if (!isSticking && !isStone)
            Move();

        if (isParachuting)
            rb.velocity = new Vector2(rb.velocity.x, -parachuteFallSpeed);

        ApplyBetterJump();

        if (lockYPosition)
        {
            transform.position = new Vector3(transform.position.x, lockedY, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
    }

    #endregion

    #region Parachute

    public void StartParachute()
    {
        isParachuting = true;
        isInvulnerable = true;
        controlsLocked = true;

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        anim?.SetTrigger(ParachuteHash);

        rb.velocity = Vector2.zero;

        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        blinkCoroutine = StartCoroutine(BlinkWhileParachuting());
    }

    IEnumerator BlinkWhileParachuting()
    {
        while (isParachuting)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        spriteRenderer.enabled = true;
    }

    void StopParachute()
    {
        isParachuting = false;
        isInvulnerable = false;
        controlsLocked = false;

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        spriteRenderer.enabled = true;
    }

    #endregion

    #region Movement

    void Move()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);

        if (moveInput.x != 0 && !isStone)
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x) * 1f, 1f, 1f);
    }

    #endregion

    #region Ground

    void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundRadius, groundLayer);

        grounded = hit.collider != null;

        if (grounded)
        {
            wasFallingStone = false;

            if (isParachuting)
                StopParachute();
        }
    }

    #endregion

    #region Jump

    void HandleJumpAssist()
    {
        coyoteCounter = grounded ? coyoteTime : coyoteCounter - Time.deltaTime;
        bufferCounter -= Time.deltaTime;

        if (bufferCounter > 0f && coyoteCounter > 0f && !isStone)
        {
            Jump();
            bufferCounter = 0f;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        coyoteCounter = 0f;
        isJumping = true;

        anim?.SetTrigger(JumpHash);
    }

    #endregion

    #region Physics

    void ApplyBetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !isJumping)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    #endregion

    #region Animator

    void UpdateAnimator()
    {
        if (!anim) return;

        float targetSpeed = Mathf.Abs(moveInput.x);
        animSpeed = Mathf.Lerp(animSpeed, targetSpeed, 0.15f);

        anim.SetFloat(SpeedHash, animSpeed);
        anim.SetBool(GroundedHash, grounded);
        anim.SetBool(StuckHash, isStuck);
        anim.SetBool(StickHash, isSticking);
    }

    #endregion

    #region Input

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isParachuting)
                StopParachute();

            bufferCounter = jumpBufferTime;
            isJumping = true;
        }

        if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            isJumping = false;
        }
    }

    public void OnStone(InputAction.CallbackContext context)
    {
        if (context.performed && !controlsLocked)
            TurnToStone();
    }

    public void OnExplode(InputAction.CallbackContext context)
    {
        if (context.performed && !controlsLocked)
            Explode();
    }

    public void OnStick(InputAction.CallbackContext context)
    {
        if (context.performed && !controlsLocked)
            StartStick();
    }

    #endregion

    #region Mechanics

    void TurnToStone()
    {
        if (isStone) return;

        isSticking = false;
        isStuck = false;
        lockYPosition = false;
        rb.gravityScale = 1f;

        isStone = true;
        isInvulnerable = true;
        canDamageEnemies = true;
        wasFallingStone = true;

        int cameraWallLayer = LayerMask.NameToLayer("CameraWall");
        Physics2D.IgnoreLayerCollision(stickingLayer, cameraWallLayer, false);

        anim?.SetTrigger(StoneHash);

        capsuleCollider.enabled = false;
        boxCollider.enabled = false;
        boxCollider2.enabled = true;

        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);

        StartCoroutine(FreezeWhenGrounded());
    }

    IEnumerator FreezeWhenGrounded()
    {
        float timer = 0f;
        float maxWait = 2f;

        while (!grounded && timer < maxWait)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        canDamageEnemies = false;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        gameObject.layer = platformLayer;
        gameObject.tag = "Platform";

        GameManager.Instance.PlayerDied();

        cameraController?.Shake(0.25f, 0.25f);

        Destroy(this);
    }

    void StartStick()
    {
        if (isStone || isStuck || isSticking) return;

        isSticking = true;
        isInvulnerable = true;
        canDamageEnemies = true;
        stuckToPlatform = false;

        gameObject.layer = stickingLayer;

        int cameraWallLayer = LayerMask.NameToLayer("CameraWall");
        Physics2D.IgnoreLayerCollision(stickingLayer, cameraWallLayer, true);

        anim?.SetTrigger(TorpedoHash);

        capsuleCollider.enabled = false;
        boxCollider.enabled = true;

        rb.gravityScale = 0f;

        float dir = Mathf.Sign(transform.localScale.x);
        rb.velocity = new Vector2(dir * 20f, 0f);

        lockedY = transform.position.y;
        lockYPosition = true;

        if (stickTimeoutCoroutine != null)
            StopCoroutine(stickTimeoutCoroutine);

        stickTimeoutCoroutine = StartCoroutine(StickTimeout());
    }

    void Stick()
    {
        isSticking = false;
        isStuck = true;
        canDamageEnemies = false;

        stuckToPlatform = true;

        lockYPosition = false;

        if (stickTimeoutCoroutine != null)
        {
            StopCoroutine(stickTimeoutCoroutine);
            stickTimeoutCoroutine = null;
        }

        gameObject.layer = platformLayer;

        anim?.SetTrigger(CrashHash);

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        rb.excludeLayers |= (1 << enemyLayer);

        GameManager.Instance.PlayerDied();

        cameraController?.Shake(0.25f, 0.25f);

        Destroy(this);
    }

    IEnumerator StickTimeout()
    {
        float t = 0f;
        float maxTime = 2f;

        while (t < maxTime)
        {
            if (stuckToPlatform)
                yield break;

            t += Time.deltaTime;
            yield return null;
        }

        // If it reaches here -> failed stick
        if (isSticking)
        {
            GameManager.Instance.PlayerDied();

            cameraController?.Shake(0.35f, 0.35f);

            Destroy(gameObject);
        }
    }

    void Explode()
    {
        if (isStone) return;

        Instantiate(explosionParticle, transform.position, Quaternion.identity);
        Instantiate(deadParticle, transform.position, Quaternion.identity);

        GameManager.Instance.PlayerDied();

        cameraController?.Shake(0.35f, 0.35f);

        Destroy(gameObject);
    }

    #endregion

    #region Collision

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int killZoneLayer = LayerMask.NameToLayer("KillZone");

        if (collision.collider.gameObject.layer == killZoneLayer)
        {
            Die();
            return;
        }

        if (!isSticking) return;

        if (collision.gameObject.layer == platformLayer)
        {
            stuckToPlatform = true;
            Stick();
        }
    }

    #endregion

    #region Death

    public void Die()
    {
        if (isInvulnerable) return;

        moveInput = Vector2.zero;
        rb.velocity = Vector2.zero;

        // Ignore Enemy layer using Rigidbody2D
        rb.excludeLayers |= (1 << enemyLayer);

        gameObject.tag = "Untagged";

        anim?.SetTrigger(DieHash);

        spriteRenderer.sortingOrder = -1;

        canDamageEnemies = false;

        GameManager.Instance.PlayerDied();

        cameraController?.Shake(0.35f, 0.35f);

        Destroy(this);
    }

    public void SetDeadBySpikes()
    {
        // Disable normal colliders
        capsuleCollider.enabled = false;
        boxCollider.enabled = false;
        boxCollider2.enabled = false;

        // Enable special corpse collider
        if (boxColliderDeadSpikes != null)
            boxColliderDeadSpikes.enabled = true;

        // Physics setup for corpse
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Visual / identity change
        gameObject.tag = "Platform";
        gameObject.layer = LayerMask.NameToLayer("Platform");

        spriteRenderer.sortingOrder = -1;

        // Optional: stop control systems
        canDamageEnemies = false;
        controlsLocked = true;
    }

    #endregion
}