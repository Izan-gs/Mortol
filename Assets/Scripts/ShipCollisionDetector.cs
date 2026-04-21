using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShipCollisionDetector : MonoBehaviour
{
    [Header("Platform Mask")]
    [SerializeField] private LayerMask platformMask;

    private Collider2D shipCollider;
    private readonly List<Collider2D> overlaps = new List<Collider2D>(8);
    private ContactFilter2D contactFilter;

    public bool IsCollidingWithPlatform { get; private set; }

    void Awake()
    {
        shipCollider = GetComponent<Collider2D>();

        if (platformMask.value == 0)
            platformMask = LayerMask.GetMask("Platform");

        contactFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = platformMask,
            useTriggers = true
        };
    }

    void Update()
    {
        if (shipCollider == null)
            return;

        overlaps.Clear();
        shipCollider.OverlapCollider(contactFilter, overlaps);
        IsCollidingWithPlatform = overlaps.Count > 0;
    }
}