using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : MonoBehaviour
{
    protected float velocity;
    protected Vector2 direction;
    protected int life;
    protected int playerLifeBonus;

    protected SpriteRenderer spriteRenderer;
    
    [HideInInspector] public UnityEvent onDie; // Event suscription

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
        Die();
    }

    protected virtual void Die()
    {
        onDie?.Invoke(); // Calls the event when self (enemy) dies
        Destroy(gameObject);
    }
}
