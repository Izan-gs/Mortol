using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpikesController : MonoBehaviour
{
    [SerializeField] private float minFallSpeedToDie = 0.0f;


    // Detects if player collides from above and then player dies
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checking if it is the player
        PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();
        if (player == null) return;
        // Checking if rigid body exists
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // Parte superior del pincho y parte inferior del jugador.
        float spikeTop = transform.position.y;
        float playerBottom = collision.bounds.min.y;

        // ¿Está cayendo?
        bool isFalling = rb.velocity.y < -minFallSpeedToDie;

        // ¿Viene desde arriba?
        bool fromAbove = playerBottom > spikeTop - 0.05f; // pequeño margen


        if (isFalling && fromAbove)
        {
            KillPlayer(player);
        }
    }

    private void KillPlayer(PlayerController player)
    {
        Debug.Log("Jugador muerto por pinchos");
        player.Die();
    }
}
