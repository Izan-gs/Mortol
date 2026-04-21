using System.Collections;
using UnityEngine;

public class PlayerExplosion : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 0.2f;
    [SerializeField] private float radius = 1f;

    [Header("Stone Effect")]
    [SerializeField] private GameObject stoneExplosionParticle;
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

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            bool isPlayer = hit.gameObject.name.Contains("Player");
            bool isPlatform = hit.CompareTag("Platform");

            if (isPlayer && isPlatform)
            {
                Instantiate(stoneExplosionParticle, hit.transform.position, Quaternion.identity);
                Destroy(hit.gameObject);
            }
        }
    }
}