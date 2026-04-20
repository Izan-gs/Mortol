using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BumblebeeController : Enemy
{
    protected override void Awake()
    {
        life = 1;
        velocity = 3;
        direction = new Vector2(1,0);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
    // 
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Otra opción de cast?
        {
            Die();
        }else if (collision.gameObject.CompareTag("Bloque"))
        {
            Die();
        }
        
    }
}