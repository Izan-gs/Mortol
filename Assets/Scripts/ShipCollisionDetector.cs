using UnityEngine;

public class ShipCollisionDetector : MonoBehaviour
{
    public bool isCollidingWithPlatform;

    private Collider2D col;
    private ContactFilter2D filter;
    private Collider2D[] results = new Collider2D[10];

    private void Awake()
    {
        col = GetComponent<Collider2D>();

        filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Platform"));
        filter.useTriggers = true;
    }

    private void FixedUpdate()
    {
        int count = col.OverlapCollider(filter, results);

        isCollidingWithPlatform = count > 0;
    }
}