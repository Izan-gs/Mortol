using UnityEngine;

public class CrocodileController : Enemy
{
    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float viewDistance = 6f;

    [Header("Vision")]
    [SerializeField] private float viewHeightTolerance = 1.5f;

    [Header("Animator")]
    [SerializeField] private float patrolAnimSpeed = 1f;
    [SerializeField] private float chaseAnimSpeed = 1.8f;

    [Header("Checks")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform edgeCheck;
    [SerializeField] private float checkDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Visual")]
    [SerializeField] private Color damagedColor = new Color(1f, 0.85f, 0.3f);

    private Transform player;
    private Animator anim;
    private bool isChasing;

    protected override void Awake()
    {
        base.Awake();

        life = 2;
        velocity = patrolSpeed;
        direction = Vector2.right;

        anim = GetComponent<Animator>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        player = p ? p.transform : null;
    }

    protected override void Update()
    {
        HandleAI();
        UpdateAnimator();
        base.Update();
    }

    #region AI

    private void HandleAI()
    {
        if (player == null || !player.CompareTag("Player"))
        {
            StopChasing();
            Patrol();
            return;
        }

        if (CanSeePlayer())
        {
            StartChasing();
        }
        else
        {
            StopChasing();
            Patrol();
        }
    }

    private bool CanSeePlayer()
    {
        Vector2 delta = player.position - transform.position;

        if (Mathf.Abs(delta.y) > viewHeightTolerance)
            return false;

        float distanceX = Mathf.Abs(delta.x);
        if (distanceX > viewDistance)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            delta.normalized,
            viewDistance,
            groundLayer
        );

        return hit.collider == null || hit.transform == player;
    }

    private void StartChasing()
    {
        if (isChasing) return;

        isChasing = true;
        velocity = chaseSpeed;

        direction = (player.position.x > transform.position.x)
            ? Vector2.right
            : Vector2.left;

        ApplyFlip();
    }

    private void StopChasing()
    {
        if (!isChasing) return;

        isChasing = false;
        velocity = patrolSpeed;
    }

    private void Patrol()
    {
        if (isChasing) return;
        DetectWallOrEdge();
    }

    #endregion

    #region Movement Logic

    // Detecta los bordes y/o paredes para proceder con el cambio de sentido
    private void DetectWallOrEdge()
    {
        bool wallHit = Physics2D.Raycast(
            wallCheck.position,
            direction,
            checkDistance,
            groundLayer
        );

        bool noGroundAhead = Physics2D.Raycast(
            edgeCheck.position,
            Vector2.down,
            checkDistance,
            groundLayer
        ).collider == null;

        if (wallHit || noGroundAhead)
            Flip();
    }

    // Aplica el cambio de signo
    private void Flip()
    {
        direction = -direction;
        ApplyFlip();
    }

    // Ajusta la escala
    private void ApplyFlip()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
        transform.localScale = scale;
    }

    #endregion

    #region Animator

    private void UpdateAnimator()
    {
        if (!anim) return;

        anim.speed = isChasing ? chaseAnimSpeed : patrolAnimSpeed;
    }

    #endregion

    #region Combat

    // Aplica el cambio de color al sprite si recibe daño
    public override void TakeDamage(int damage)
    {
        // 
        base.TakeDamage(damage);

        if (spriteRenderer != null && life == 1)
        {
            spriteRenderer.color = damagedColor;
        }
    }

    #endregion
}