using System;
//using UnityEditor.Playables;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

[RequireComponent(typeof(LineRenderer))]
public class LaserScript : MonoBehaviour
{
    
    [Header("Laser Settings")]
    [Tooltip("Maximum number of reflections before stopping.")]
    public int maxReflections = 5;
    [Tooltip("Maximum distance for each raycast segment.")]
    public float maxDistance = 100f;

    [Header("Laser Color")]
    public CustomColors LaserColor = CustomColors.white ;

    [Header("Layer Mask")]
    [Tooltip("Layer mask for objects the laser can hit (Mirrors, Splitters, etc.).")]
    public LayerMask reflectionMask;

    // Internal color used by the laser
    //private CustomColors LaserColor = CustomColors.white;
    // Flag to prevent Start() from overriding externally set color.
    private bool initializedExternally = false;

    // Cached LineRenderer
    private LineRenderer lineRenderer;
    
    // Events to inform the splitter it was hit/not-hit by lazer
    public event Action<Vector2,Vector2,Color> SplitterCollisionEnterEvent;
    public event Action SplitterCollisionExitEvent;
    private bool SplitterActivated;
    LaserSplitter SplitterOnContact = null; 
    
    //Material and colors
    [SerializeField] private Material RedMat;
    [SerializeField] private Material BlueMat;
    [SerializeField] private Material GreenMat;
    [SerializeField] private Material WhiteMat;
    
    /// <summary>
    /// Public getter for the laser's color (used by external scripts, e.g. ColorReceiver).
    /// </summary>
    //public Color LaserColor => laserColor;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        switch (LaserColor)
        {
            case CustomColors.white://White
                lineRenderer.materials = new[] { WhiteMat };
                break;
            case CustomColors.red://red
                lineRenderer.materials = new[] { RedMat };
                break; 
            case CustomColors.green://green 
                lineRenderer.materials = new[] { GreenMat };
                break; 
            case CustomColors.blue://blue 
                lineRenderer.materials = new[] { BlueMat };
                break; 
        }

        // Ensure the LineRenderer has an unlit material (avoids purple or gray color).
        // if (lineRenderer.material == null)
        // {
        //     Material defaultMat = new Material(Shader.Find("Unlit/Color"));
        //     defaultMat.color = Color.white;  // default to white
        //     lineRenderer.material = defaultMat;
        // }
    }

    private void Start()
    {
        SplitterActivated = false; 
        // Only parse the hex color if not already set externally
        if (!initializedExternally)
        {
            // Set the final laser color on the LineRenderer
            SetLaserColor(LaserColor);
        }
    }

    private void Update()
    {
        DrawLaser();
    }

    /// <summary>
    /// Sets the color of the laser (LineRenderer start/end + material) and marks it as externally initialized.
    /// </summary>
    public void SetLaserColor(CustomColors newColor)
    {
        LaserColor = newColor;
        initializedExternally = true;

        if (lineRenderer != null)
        {
            lineRenderer.startColor = CustomColorsUtility.CustomColorToUnityColor(LaserColor);
            lineRenderer.endColor = CustomColorsUtility.CustomColorToUnityColor(LaserColor);
        }

        switch (LaserColor)
        {
            case CustomColors.white://White
                lineRenderer.materials = new[] { WhiteMat };
                break;
            case CustomColors.red://red
                lineRenderer.materials = new[] { RedMat };
                break; 
            case CustomColors.green://green 
                lineRenderer.materials = new[] { GreenMat };
                break; 
            case CustomColors.blue://blue 
                lineRenderer.materials = new[] { BlueMat };
                break; 
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
        bool reachedSplitter = false;
        
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
                    receiver.LaserHitting(LaserColor);
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
                else if (hit.collider.CompareTag("Splitter") || hit.collider.CompareTag("Splitter_RGB") )
                {
                    Debug.Log("Laser hit a splitter. Calling SplitLaser.");
                    SplitterOnContact = hit.collider.GetComponent<LaserSplitter>();
                    if (SplitterOnContact != null)
                    {
                        // if (SplitterCollisionEnterEvent is not null)
                        // {
                        //     splitterOnContact = hit.collider.gameObject;
                        //     SplitterActivated = true;
                        //     reachedSplitter = true; 
                        //     SplitterCollisionEnterEvent(hit.point, rayDirection,laserColor);
                        // }
                        SplitterActivated = true;
                        reachedSplitter = true; 
                        SplitterOnContact.SplitLaser(hit.point, rayDirection,LaserColor);
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
        
        if (SplitterActivated && !reachedSplitter)
        {
            SplitterOnContact.DestroyAllEmittingLazers();
                //SplitterCollisionExitEvent();
                SplitterActivated = false;
                SplitterOnContact = null; 
        }
    }
}
