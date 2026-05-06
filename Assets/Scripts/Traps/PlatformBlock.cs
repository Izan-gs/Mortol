using UnityEngine;

public class PlatformBlock : MonoBehaviour
{
    public ActivatorBlock.ActivatorType type;

    [Header("Default State")]
    [SerializeField] private bool startActive = true;

    [Header("Sprites")]
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    private SpriteRenderer sr;
    private Collider2D col;

    private bool isActive;

    [SerializeField] private LayerMask enemyLayer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        isActive = startActive;
        ApplyState();
    }

    private void OnEnable()
    {
        ActivatorBlock.OnActivatorChanged += HandleActivator;
    }

    private void OnDisable()
    {
        ActivatorBlock.OnActivatorChanged -= HandleActivator;
    }

    private void HandleActivator(ActivatorBlock.ActivatorType t, bool pressed)
    {
        if (t != type) return;

        // Invert based on press + initial state
        isActive = startActive ? !pressed : pressed;

        ApplyState();
    }

    private void ApplyState()
    {
        if (col != null)
            col.enabled = isActive;

        if (sr != null)
            sr.sprite = isActive ? activeSprite : inactiveSprite;

        if (isActive)
        {
            KillEnemiesInside();
        }
    }

    private void KillEnemiesInside()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            col.bounds.center,
            col.bounds.size,
            0f,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(999, false);
            }
        }
    }
}