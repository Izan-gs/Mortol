using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigController : Enemy
{
    protected override void Awake()
    {
        life = 1;
        velocity = 3;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bloque"))
        {
            direction *= -1; // Changing the direction when hitting a block
        }

    }
}
