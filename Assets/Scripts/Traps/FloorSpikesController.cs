using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpikesController : MonoBehaviour
{
    // Detects if player collides from above and then player dies
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            player.Die();
        }
    }
}
