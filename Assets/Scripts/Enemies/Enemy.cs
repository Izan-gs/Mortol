using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : MonoBehaviour
{
    protected float velocity;
    protected Vector2 direction;
    protected int life;
    protected int playerLifeBonus;

    protected SpriteRenderer spriteRenderer;

    [Header("FX")]
    [SerializeField] private GameObject deadParticle;

    [HideInInspector] public UnityEvent onDie; // Event suscription

    private bool isInvulnerable = false;
    [SerializeField] private float invulnerableTime = 0.1f;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        Move();
    }

    protected void Move()
    {
        transform.Translate(direction * velocity * Time.deltaTime);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();

        if (player != null && player.canDamageEnemies)
        {
            TakeDamage(1);
            return;
        }
        else if (player != null)
        {
            player.Die();
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        life -= damage;

        if (life <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(Invulnerability());
    }

    private IEnumerator Invulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerableTime);
        isInvulnerable = false;
    }


    protected virtual void Die()
    {
        onDie?.Invoke();

        if (deadParticle != null)
        {
            Instantiate(deadParticle, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}