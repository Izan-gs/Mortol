using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocodileController : Enemy
{
    private float assaultSpeed;

    public void Assault()
    {
        velocity = assaultSpeed;
    }
    //
    protected override void Awake()
    {
        life = 3; // Por ritual stone muere de 1 hit
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
}
