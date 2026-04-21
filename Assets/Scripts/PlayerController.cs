using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Colliders")]
    public CapsuleCollider2D capsuleCollider;
    public BoxCollider2D boxCollider;
    public BoxCollider2D boxCollider2;
    private BoxCollider2D cameraWallCollider;

    [Header("FX")]
    public GameObject explosionParticle;

    #region Movement
    [SerializeField] private float moveSpeed = 5f;
    private float lockedY;
    private bool lockYPosition;
    private Vector2 moveInput;
    #endregion

    #region Jump
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    private bool isJumping;
    #endregion

    #region Jump Assist
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    private float coyoteCounter;
    private float bufferCounter;
    #endregion

    #region Ground
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
    public bool canDamageEnemies = false;
    private bool controlsLocked;
    #endregion

    #region Parachute
    [SerializeField] private float parachuteFallSpeed = 0.7f;
    private bool isParachuting;
    private Coroutine blinkCoroutine;
    [SerializeField] private float blinkInterval = 0.1f;
    #endregion

    [Header("FX")]
    [SerializeField] private GameObject deadParticle;

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

    private float animSpeed;

    public bool IsSticking() => isSticking;
    public bool IsStoneFalling() => isStone && wasFallingStone;
    public bool isInvulnerable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

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

        if (!isSticking)
            Move();

        if (isParachuting)
        {
            rb.velocity = new Vector2(rb.velocity.x, -parachuteFallSpeed);
        }

        ApplyBetterJump();

        if (lockYPosition)
        {
            transform.position = new Vector3(
                transform.position.x,
                lockedY,
                transform.position.z
            );

            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
    }

    public void StartParachute()
    {
        isParachuting = true;
        isInvulnerable = true;
        controlsLocked = true;

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        if (anim) anim.SetTrigger(ParachuteHash);

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

    #region Movement
    void Move()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);

        if (moveInput.x != 0 && !isStone)
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x) * 5f, 5f, 5f);
    }
    #endregion

    #region Ground & Jump
    void CheckGround()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (grounded)
        {
            wasFallingStone = false;

            if (isParachuting)
            {
                isParachuting = false;
                isInvulnerable = false;
                controlsLocked = false;

                int playerLayer = LayerMask.NameToLayer("Player");
                int enemyLayer = LayerMask.NameToLayer("Enemy");

                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

                if (blinkCoroutine != null)
                {
                    StopCoroutine(blinkCoroutine);
                    blinkCoroutine = null;
                }

                spriteRenderer.enabled = true;
            }
        }
    }

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

        if (anim) anim.SetTrigger(JumpHash);
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
            {
                isParachuting = false;
                isInvulnerable = false;
                controlsLocked = false;

                int playerLayer = LayerMask.NameToLayer("Player");
                int enemyLayer = LayerMask.NameToLayer("Enemy");

                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

                if (blinkCoroutine != null)
                {
                    StopCoroutine(blinkCoroutine);
                    blinkCoroutine = null;
                }

                spriteRenderer.enabled = true;
            }

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

        isStone = true;
        isSticking = false;
        isInvulnerable = true;

        canDamageEnemies = true;

        wasFallingStone = true;

        if (anim) anim.SetTrigger(StoneHash);

        capsuleCollider.enabled = false;
        boxCollider2.enabled = true;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);

        StartCoroutine(FreezeWhenGrounded());
    }

    IEnumerator FreezeWhenGrounded()
    {
        while (!grounded)
            yield return null;

        yield return new WaitForSeconds(0.05f);

        canDamageEnemies = false;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        gameObject.layer = LayerMask.NameToLayer("Platform");
        gameObject.tag = "Platform";
    }

    void StartStick()
    {
        if (isStone || isStuck || isSticking) return;

        isSticking = true;
        isJumping = false;
        isInvulnerable = true;

        canDamageEnemies = true;

        gameObject.layer = LayerMask.NameToLayer("PlayerSticking");

        if (anim) anim.SetTrigger(TorpedoHash);

        capsuleCollider.enabled = false;
        boxCollider.enabled = true;

        rb.gravityScale = 0f;

        float dir = Mathf.Sign(transform.localScale.x);
        rb.velocity = new Vector2(dir * 20f, 0f);

        lockedY = transform.position.y;
        lockYPosition = true;
    }

    void Stick()
    {
        isSticking = false;
        isStuck = true;

        canDamageEnemies = false;

        lockYPosition = false;

        gameObject.layer = LayerMask.NameToLayer("Player");

        if (anim) anim.SetTrigger(CrashHash);

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        Destroy(this);
    }

    void Explode()
    {
        Instantiate(explosionParticle, transform.position, Quaternion.identity);
        Instantiate(deadParticle, transform.position, Quaternion.identity);

        GameManager.Instance.PlayerDied();

        Destroy(gameObject);
    }
    #endregion

    #region Collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isSticking) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
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

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int otherPlayerLayer = LayerMask.NameToLayer("Player");

        // Add layers to Rigidbody2D exclusion list
        rb.excludeLayers = (1 << enemyLayer) | (1 << otherPlayerLayer);

        gameObject.tag = "Untagged";

        if (anim) anim.SetTrigger(DieHash);

        spriteRenderer.sortingOrder = -1;

        canDamageEnemies = false;

        GameManager.Instance.PlayerDied();

        Destroy(this);
    }
    #endregion
}