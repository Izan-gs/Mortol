using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumblebeeController : Enemy
{
    private NestController nest;

    protected override void Awake()
    {
        life = 1;
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
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            nest.IncrementDeadUnits();
            Die();
        }else if (collision.gameObject.CompareTag("Bloque"))
        {
            Die();
        }
        
    }
}
