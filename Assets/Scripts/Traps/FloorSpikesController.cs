using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpikesController : MonoBehaviour
{
    [SerializeField] private float minFallSpeedToDie = 0.0f;
    [SerializeField] private GameObject destroyParticle;

    // Detects if player collides from above and then player dies
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float spikeTop = transform.position.y;
        float playerBottom = collision.bounds.min.y;

        bool isFalling = rb.velocity.y < -minFallSpeedToDie;
        bool fromAbove = playerBottom > spikeTop - 0.05f;

        // STONE FALLING -> DESTROY SPIKE
        if (player.IsStoneFalling() && isFalling && fromAbove)
        {
            Instantiate(destroyParticle, transform.position, Quaternion.identity);

            Destroy(gameObject); // spike breaks
            return;
        }

        // NORMAL CASE -> KILL PLAYER
        if (isFalling && fromAbove)
        {
            KillPlayer(player);
        }
    }

    private void KillPlayer(PlayerController player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        player.transform.position = new Vector3(
            player.transform.position.x,
            transform.position.y + 0.7f,
            player.transform.position.z
        );

        // SPECIAL SPIKE DEATH STATE
        player.SetDeadBySpikes();

        player.Die();
    }
}
