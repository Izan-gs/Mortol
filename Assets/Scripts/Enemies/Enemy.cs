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
    protected int playerLifeBonus = 1;

    protected SpriteRenderer spriteRenderer;

    [Header("FX")]
    [SerializeField] private GameObject deadParticle;
    public GameObject lifesAddEffect;

    [HideInInspector] public UnityEvent onDie;

    private bool isInvulnerable = false;
    protected bool killedByPlayerSource = false;
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

    public virtual void TakeDamage(int damage, bool fromPlayer = false)
    {
        if (isInvulnerable) return;

        if (fromPlayer)
            killedByPlayerSource = true;

        life -= damage;

        if (life <= 0)
        {
            // Randomize life bonus between 1 and 2
            playerLifeBonus = Random.Range(1, 3);

            // Spawn life effect
            if (lifesAddEffect != null)
            {
                GameObject effectInstance = Instantiate(lifesAddEffect, transform.position, Quaternion.identity);

                // Get TMP_Text from children and set value
                TMPro.TMP_Text text = effectInstance.GetComponentInChildren<TMPro.TMP_Text>();
                if (text != null)
                {
                    text.text = playerLifeBonus.ToString();
                }
            }

            // Add life to player
            GameManager.Instance.playerLives += playerLifeBonus;

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

        // Spawn death particles
        if (deadParticle != null)
        {
            Instantiate(deadParticle, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}