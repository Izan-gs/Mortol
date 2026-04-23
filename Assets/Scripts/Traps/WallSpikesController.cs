using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpikesController : MonoBehaviour
{
    // Detects if player collides from above and then player dies
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        KillPlayer(player);
    }

    private void KillPlayer(PlayerController player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // SPECIAL SPIKE DEATH STATE
        player.SetDeadBySpikes();

        player.Die();
    }
}
