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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
