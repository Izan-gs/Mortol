using System.Collections;
using UnityEngine;

public class PlayerExplosion : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 0.2f;
    [SerializeField] private float radius = 1f;

    [Header("Stone Effect")]
    [SerializeField] private GameObject stoneExplosionParticle;

    [Header("Brick Effect")]
    [SerializeField] private GameObject brickExplosionParticle;

    [SerializeField] private LayerMask targetLayer;

    private bool hasExploded;

    private void Start()
    {
        Explode();
        Destroy(gameObject, lifeTime);
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Ensure Enemy layer is included
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int combinedLayer = targetLayer | (1 << enemyLayer);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, combinedLayer);

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, true);
            }

            bool isPlayer = hit.gameObject.name.Contains("Player");
            bool isPlatform = hit.CompareTag("Platform");

            if (isPlayer && isPlatform)
            {
                Instantiate(stoneExplosionParticle, hit.transform.position, Quaternion.identity);
                Destroy(hit.gameObject);
            }

            // Brick detection
            if (hit.gameObject.name.Contains("Brick"))
            {
                if (brickExplosionParticle != null)
                {
                    Instantiate(brickExplosionParticle, hit.transform.position, Quaternion.identity);
                }

                Destroy(hit.gameObject);
            }
        }
    }
}