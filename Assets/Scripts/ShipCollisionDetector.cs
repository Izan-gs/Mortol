using UnityEngine;

public class ShipCollisionDetector : MonoBehaviour
{
    public bool isCollidingWithPlatform;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            isCollidingWithPlatform = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            isCollidingWithPlatform = false;
        }
    }
}