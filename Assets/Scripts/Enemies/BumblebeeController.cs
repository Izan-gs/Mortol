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
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null && player.IsStoneFalling())
            {
                killedByPlayer = true;
                Die();
            }
        }
        else if (collision.gameObject.CompareTag("Platform"))
        {
            Die();
        }
    }

    protected override void Die()
    {
        if (killedByPlayer && nestObjRef != null)
        {
            nestObjRef.GetComponent<NestController>().IncrementDeadUnits();
        }

        base.Die();
    }
}