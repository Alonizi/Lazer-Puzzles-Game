using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserScript : MonoBehaviour
{
    [Header("Laser Settings")]
    [Tooltip("Maximum number of reflections before stopping.")]
    public int maxReflections = 5;
    [Tooltip("Maximum distance for each raycast segment.")]
    public float maxDistance = 100f;

    [Header("Laser Color (Hex)")]
    [Tooltip("Hex color for the laser, e.g. #FFFFFF for white.")]
    public string hexColor = "#FFFFFF";

    [Header("Layer Mask")]
    [Tooltip("Layer mask for objects the laser can hit (Mirrors, Splitters, etc.).")]
    public LayerMask reflectionMask;

    // Internal color used by the laser
    private Color laserColor = Color.white;

    // Cached LineRenderer
    private LineRenderer lineRenderer;

    /// <summary>
    /// Public getter for the laser's color (used by external scripts, e.g. ColorReceiver).
    /// </summary>
    public Color LaserColor => laserColor;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Ensure the LineRenderer has an unlit material (avoids purple or gray color).
        if (lineRenderer.material == null)
        {
            Material defaultMat = new Material(Shader.Find("Unlit/Color"));
            defaultMat.color = Color.white;  // default to white
            lineRenderer.material = defaultMat;
        }
    }

    private void Start()
    {
        // Parse the hex color string
        if (ColorUtility.TryParseHtmlString(hexColor, out Color parsedColor))
        {
            laserColor = parsedColor;
        }
        else
        {
            Debug.LogWarning($"Invalid hex color \"{hexColor}\". Defaulting to white.");
            laserColor = Color.white;
        }

        // Set the final laser color on the LineRenderer
        SetLaserColor(laserColor);
    }

    private void Update()
    {
        DrawLaser();
    }

    /// <summary>
    /// Sets the color of the laser (LineRenderer start/end + material).
    /// </summary>
    public void SetLaserColor(Color newColor)
    {
        laserColor = newColor;

        if (lineRenderer != null)
        {
            lineRenderer.startColor = newColor;
            lineRenderer.endColor = newColor;

            // Also set the material color so it appears correctly unlit
            if (lineRenderer.material != null)
            {
                lineRenderer.material.color = newColor;
            }
        }
    }

    /// <summary>
    /// Casts and draws the laser, reflecting on mirrors or splitting if it hits a splitter.
    /// </summary>
    private void DrawLaser()
    {
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = transform.right;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, rayOrigin);

        int reflections = 0;

        while (reflections < maxReflections)
        {
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, maxDistance, reflectionMask);
            if (hit.collider != null)
            {
                // Add the hit point
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                // Check if the hit object is a SimpleColorReceiver
                SimpleColorReceiver receiver = hit.collider.GetComponent<SimpleColorReceiver>();
                if (receiver != null)
                {
                    // Send the laser color to the receiver and notify it's being hit
                    receiver.LaserHitting(laserColor);
                }
                else
                {
                    // Reset LaserStopped method if we stop hitting a receiver
                    receiver = hit.collider.GetComponent<SimpleColorReceiver>();
                    if (receiver != null)
                    {
                        receiver.LaserStopped();
                    }
                }

                // Handle reflections for mirrors and splitters
                if (hit.collider.CompareTag("Mirror"))
                {
                    rayDirection = Vector2.Reflect(rayDirection, hit.normal);
                    rayOrigin = hit.point + rayDirection * 0.01f;
                }
                else if (hit.collider.CompareTag("Splitter"))
                {
                    // Handle splitters (optional logic)
                }
                else
                {
                    break; // Stop if it hits anything else
                }
            }
            else
            {
                // Extend the laser to maxDistance if no hit
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, rayOrigin + rayDirection * maxDistance);
                break;
            }

            reflections++;
        }
    }
}
