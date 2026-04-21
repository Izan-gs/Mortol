using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpikesController : MonoBehaviour
{
    [SerializeField] private float minFallSpeedToDie = 0.1f;

    // Detects if player collides from above and then player dies
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // Obtenemos SOLO el primer contacto
        ContactPoint2D contact = collision.GetContact(0);

        // Condiciones tipo Mortol
        bool hitFromAbove = contact.normal.y > 0.5f;
        bool isFalling = rb.velocity.y < -minFallSpeedToDie;

        if (hitFromAbove && isFalling)
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
