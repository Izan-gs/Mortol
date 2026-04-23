using System.Collections;
using UnityEngine;

public class BumblebeeController : Enemy
{
    private Rigidbody2D rb;
    private Collider2D beeCollider;
    private Collider2D nestCollider;
    private GameObject nestObjRef;

    private bool killedByPlayer = false;

    protected override void Awake()
    {
        base.Awake();

        life = 1;
        velocity = 8;

        rb = GetComponent<Rigidbody2D>();
        beeCollider = GetComponent<Collider2D>();
    }

    public void Initialize(Vector2 spawnDirection, GameObject nestObj)
    {
        direction = spawnDirection.normalized;

        nestObjRef = nestObj;
        nestCollider = nestObj.GetComponent<Collider2D>();

        if (nestCollider != null)
        {
            Physics2D.IgnoreCollision(beeCollider, nestCollider, true);
        }

        StartCoroutine(SpawnImmunity());
    }

    private IEnumerator SpawnImmunity()
    {
        yield return new WaitForSeconds(1f);

        if (nestCollider != null)
        {
            Physics2D.IgnoreCollision(beeCollider, nestCollider, false);
        }
    }

    protected override void Update()
    {
        rb.velocity = direction * velocity;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            // If player is in a "killable state"
            if (player.IsStoneFalling() || player.IsSticking())
            {
                killedByPlayerSource = true;

                playerLifeBonus = Random.Range(1, 3);

                if (lifesAddEffect != null)
                {
                    GameObject effectInstance = Instantiate(lifesAddEffect, transform.position, Quaternion.identity);

                    TMPro.TMP_Text text = effectInstance.GetComponentInChildren<TMPro.TMP_Text>();
                    if (text != null)
                        text.text = playerLifeBonus.ToString();
                }

                GameManager.Instance.playerLives += playerLifeBonus;

                Die();
            }
            else
            {
                player.Die();
            }

            return;
        }

        if (collision.gameObject.CompareTag("Platform"))
        {
            Die();
        }
    }

    protected override void Die()
    {
        if (nestObjRef != null)
        {
            NestController nest = nestObjRef.GetComponent<NestController>();
            if (nest != null)
            {
                nest.IncrementDeadUnits(killedByPlayerSource);
            }
        }

        base.Die();
    }
}