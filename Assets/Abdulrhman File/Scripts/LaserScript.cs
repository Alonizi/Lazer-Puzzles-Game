using UnityEngine;

public class LaserScript : MonoBehaviour
{
    [Header("Laser Settings")]
    [Tooltip("Maximum number of times the laser can reflect.")]
    public int maxReflections = 5;
    [Tooltip("Maximum distance for each raycast.")]
    public float maxDistance = 100f;
    
    [Header("References")]
    [Tooltip("LineRenderer component used to draw the laser.")]
    public LineRenderer lineRenderer;
    [Tooltip("Layer mask for objects the laser can hit (e.g., mirrors and reactor).")]
    public LayerMask reflectionMask;

    private void Update()
    {
        DrawLaser();
    }

    /// <summary>
    /// Casts a ray and draws the laser with reflections.
    /// </summary>
    void DrawLaser()
    {
        // Start from the emitter's position.
        Vector2 rayOrigin = transform.position;
        // Laser shoots from the object's right side; adjust if needed.
        Vector2 rayDirection = transform.right;

        // Initialize the LineRenderer positions.
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, rayOrigin);

        int reflections = 0;

        // Continue reflecting while we haven't reached the max count.
        while (reflections < maxReflections)
        {
            // Cast the ray.
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, maxDistance, reflectionMask);
            if (hit)
            {
                // Add the hit point to the LineRenderer.
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                // Check if the hit object is a mirror.
                if (hit.collider.CompareTag("Mirror"))
                {
                    // Reflect the direction using the hit normal.
                    rayDirection = Vector2.Reflect(rayDirection, hit.normal);
                    // Offset the ray origin slightly to avoid immediate re-hit.
                    rayOrigin = hit.point + rayDirection * 0.01f;
                }
                else
                {
                    // If it hit a non-mirror object (like the reactor), stop the laser.
                    break;
                }
            }
            else
            {
                // If no hit, extend the ray to max distance.
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, rayOrigin + rayDirection * maxDistance);
                break;
            }
            reflections++;
        }
    }
}
