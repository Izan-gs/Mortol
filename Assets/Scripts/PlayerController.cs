using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public CapsuleCollider2D capsuleCollider;
    public BoxCollider2D boxCollider;
    public BoxCollider2D boxCollider2;

    public GameObject explosionParticle;

    #region Movement
    [SerializeField] private float moveSpeed = 5f;
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
    private bool isStuck;
    private bool isSticking;
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
    #endregion

    private float animSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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

        ApplyBetterJump();
    }

    #region Animator Safe Update

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

    #region Ground + Jump
    void CheckGround()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
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

    #region Better Jump
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
        => moveInput = context.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
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
        if (context.performed)
            TurnToStone();
    }

    public void OnExplode(InputAction.CallbackContext context)
    {
        if (context.performed)
            Explode();
    }

    public void OnStick(InputAction.CallbackContext context)
    {
        if (context.performed)
            StartStick();
    }
    #endregion

    #region Mechanics
    void TurnToStone()
    {
        if (isStone) return;

        isStone = true;
        isSticking = false;

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

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }

    void Explode()
    {
        Instantiate(explosionParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void StartStick()
    {
        if (isStone || isStuck || isSticking) return;

        isSticking = true;
        isJumping = false;

        if (anim) anim.SetTrigger(TorpedoHash);

        capsuleCollider.enabled = false;
        boxCollider.enabled = true;

        rb.gravityScale = 0f;

        float dir = Mathf.Sign(transform.localScale.x);
        rb.velocity = new Vector2(dir * 15f, 0f);
    }

    void Stick()
    {
        isSticking = false;
        isStuck = true;

        if (anim) anim.SetTrigger(CrashHash);

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
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
}