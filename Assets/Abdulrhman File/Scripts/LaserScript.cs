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
    // Flag to prevent Start() from overriding externally set color.
    private bool initializedExternally = false;

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
        // Only parse the hex color if not already set externally
        if (!initializedExternally)
        {
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
    }

    private void Update()
    {
        DrawLaser();
    }

    /// <summary>
    /// Sets the color of the laser (LineRenderer start/end + material) and marks it as externally initialized.
    /// </summary>
    public void SetLaserColor(Color newColor)
    {
        laserColor = newColor;
        initializedExternally = true;

        if (lineRenderer != null)
        {
            lineRenderer.startColor = newColor;
            lineRenderer.endColor = newColor;
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
                Debug.Log($"Laser hit: {hit.collider.name} (Tag: {hit.collider.tag}) at {hit.point}");
                // Add the hit point to the line renderer.
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                // Check for a SimpleColorReceiver.
                SimpleColorReceiver receiver = hit.collider.GetComponent<SimpleColorReceiver>();
                if (receiver != null)
                {
                    receiver.LaserHitting(laserColor);
                }
                else
                {
                    receiver = hit.collider.GetComponent<SimpleColorReceiver>();
                    if (receiver != null)
                    {
                        receiver.LaserStopped();
                    }
                }

                // Handle collisions.
                if (hit.collider.CompareTag("Mirror"))
                {
                    rayDirection = Vector2.Reflect(rayDirection, hit.normal);
                    rayOrigin = hit.point + rayDirection * 0.01f;
                }
                else if (hit.collider.CompareTag("Splitter"))
                {
                    Debug.Log("Laser hit a splitter. Calling SplitLaser.");
                    LaserSplitter splitter = hit.collider.GetComponent<LaserSplitter>();
                    if (splitter != null)
                    {
                        splitter.SplitLaser(hit.point, rayDirection);
                    }
                    else
                    {
                        Debug.LogWarning("LaserSplitter component not found on the splitter object.");
                    }
                    break; // Stop processing after splitting.
                }
                else
                {
                    break;
                }
            }
            else
            {
                Debug.Log("Laser did not hit any collider.");
                // Extend laser to maxDistance.
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, rayOrigin + rayDirection * maxDistance);
                break;
            }
            reflections++;
        }
    }
}
