using UnityEngine;

public class PigController : Enemy
{
    [Header("Checks")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform edgeCheck;
    [SerializeField] private float checkDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    protected override void Awake()
    {
        base.Awake();

        life = 1;
        velocity = 3f;
        direction = Vector2.right;
    }

    protected override void Update()
    {
        HandleMovement();
        base.Update();
    }

    #region Movement Logic

    private void HandleMovement()
    {
        if (ShouldFlip())
            Flip();
    }

    private bool ShouldFlip()
    {
        bool wallAhead = Physics2D.Raycast(
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

        return wallAhead || noGroundAhead;
    }

    private void Flip()
    {
        direction = -direction;
        ApplyFlip();
    }

    private void ApplyFlip()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
        transform.localScale = scale;
    }

    #endregion
}