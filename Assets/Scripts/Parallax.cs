using UnityEngine;

public class Parallax : MonoBehaviour
{
    Transform cam;
    Vector3 previousCamPos;
    Vector3 initialCamOffset;
    Vector3 startPosition;

    public float distanceX = 0f;
    public float distanceY = 0f;

    public float smoothingX = 1f;
    public float smoothingY = 1f;

    void Awake()
    {
        cam = Camera.main.transform;
        startPosition = transform.position;
        initialCamOffset = startPosition - cam.position;
        previousCamPos = cam.position;
    }

    void Update()
    {
        Vector3 newPosition = transform.position;

        if (distanceX != 0f)
        {
            float parallaxX = (cam.position.x - previousCamPos.x) * distanceX;
            newPosition.x = Mathf.Lerp(transform.position.x, transform.position.x + parallaxX, smoothingX * Time.deltaTime);
        }

        if (distanceY != 0f)
        {
            float parallaxY = (cam.position.y - previousCamPos.y) * distanceY;
            newPosition.y = Mathf.Lerp(transform.position.y, transform.position.y + parallaxY, smoothingY * Time.deltaTime);
        }

        transform.position = newPosition;
        previousCamPos = cam.position;
    }

    public void ResetParallax()
    {
        transform.position = cam.position + initialCamOffset;
        previousCamPos = cam.position;
    }
}