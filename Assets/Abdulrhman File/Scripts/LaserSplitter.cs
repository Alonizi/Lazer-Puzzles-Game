using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Splits an incoming white laser beam into three primary RGB beams (Red, Green, Blue).
/// The beam that hits the splitter determines which side is blocked; the new beams exit
/// from the remaining three sides.
/// </summary>
public class LaserSplitter : MonoBehaviour
{
    [Tooltip("Prefab of the laser emitter (must have LaserScript + LineRenderer).")]
    public GameObject laserPrefab;

    [Tooltip("Distance from splitter center to spawn new beams.")]
    public float spawnOffset = 0.5f;
    
    [Tooltip("Set if the Splitter should match or split the received color")]
    [SerializeField] private bool MatchColors;
    
    [Header("Red-Green-Blue")]
    [Tooltip("Set the color-hex values of the 3 primary colors (Red-Green-Blue) in order.")]
    private CustomColors[] PrimaryColors = { CustomColors.red, CustomColors.green, CustomColors.blue };

    // Flag to ensure we only split once per hit.
    private bool hasSplit = false;

    private List<GameObject> EmittedLazers;

    [SerializeField] private AudioSource SplitterSFX; 
    [SerializeField] private Light2D SingleColorSplitterLight; 
    private float LightTimer = 0;
    private float LightInitialIntensity;

    private CustomColors IncomingColor = CustomColors.white; 

    private void Awake()
    {
        EmittedLazers = new List<GameObject>();
    }

    private void Update()
    {
        LightTimer += Time.deltaTime;
        if (MatchColors)
        {
            SingleColorSplitterLightToggle(IncomingColor);
        }

    }

    /// <summary>
    /// Splits an incoming white laser into three beams: red, green, blue.
    /// New beams exit from the three sides not directly hit.
    /// </summary>
    /// <param name="hitPoint">World point where the laser hit the splitter.</param>
    /// <param name="incomingDirection">Direction from which the laser is coming.</param>
    /// <param name="incomingLazerColor">Color of the laser hitting the splitter.</param>
    public void SplitLaser(Vector2 hitPoint, Vector2 incomingDirection, CustomColors incomingLazerColor)
    {
        Debug.LogWarning("Splitter Event Activated");

        // Only allow splitting with a white beam when MatchColors is disabled.
        if (!MatchColors && incomingLazerColor != CustomColors.white)
        {
            Debug.Log("Non-white beam received. Splitting aborted since only white beams are accepted.");
            return;
        }

        if (hasSplit)
        {
            Debug.Log("Splitter has already split the laser. Skipping further splits.");
            return;
        }
        hasSplit = true;

        Debug.Log($"Splitter activated. Hit point: {hitPoint}, Incoming direction: {incomingDirection}");

        // Convert hitPoint to local space.
        Vector2 localHit = transform.InverseTransformPoint(hitPoint);
        Debug.Log("Local hit position: " + localHit);

        // Determine which side was hit.
        Vector2 hitSide;
        if (Mathf.Abs(localHit.x) > Mathf.Abs(localHit.y))
        {
            hitSide = localHit.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            hitSide = localHit.y > 0 ? Vector2.up : Vector2.down;
        }
        Debug.Log("Hit side determined as: " + hitSide);
        
        // Define the four cardinal directions.
        Vector2[] sides = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

        // Exclude the side that was hit.
        List<Vector2> outputSides = new List<Vector2>();
        foreach (Vector2 side in sides)
        {
            if (Vector2.Dot(side, hitSide) > 0.9f)
            {
                Debug.Log("Skipping side: " + side + " as it matches hit side.");
                continue;
            }
            outputSides.Add(side);
        }

        // Spawn one laser for each output side.
        for (int i = 0; i < outputSides.Count; i++)
        {
            Vector2 localSide = outputSides[i];
            Vector2 worldDir = transform.TransformDirection(localSide).normalized;
            Vector2 spawnPos = (Vector2)transform.position + worldDir * spawnOffset;
            Debug.Log($"Spawning laser {i} at {spawnPos} with direction {worldDir}");

            GameObject newLaser = Instantiate(laserPrefab, spawnPos, Quaternion.identity);

            float angle = Mathf.Atan2(worldDir.y, worldDir.x) * Mathf.Rad2Deg;
            newLaser.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            LaserScript laserScript = newLaser.GetComponent<LaserScript>();
            
            if (laserScript != null)
            {
                if (MatchColors)
                {
                    laserScript.SetLaserColor(incomingLazerColor);
                    IncomingColor = incomingLazerColor;
                    transform.GetChild(0).GetComponent<SpriteRenderer>().color = CustomColorsUtility.CustomColorToUnityColor(incomingLazerColor);
                    //SingleColorSplitterLightToggle(IncomingColor);
                }
                else
                {
                    laserScript.SetLaserColor(PrimaryColors[i % PrimaryColors.Length]);
                }
                EmittedLazers.Add(newLaser);
            }
            else
            {
                Debug.LogWarning("LaserScript component not found on the instantiated laser prefab.");
            }
        }

        if (hasSplit)
        {
            SplitterSFX.Play();
        }
    }

    private void OnDestroy()
    {
        DestroyAllEmittingLazers();
    }

    public void DestroyAllEmittingLazers()
    {
        Debug.LogWarning("Splitter Exit Collision Event");
        foreach (var laser in EmittedLazers)
        {
            Destroy(laser);
        }
        hasSplit = false; 
        IncomingColor = CustomColors.white; 
    }

    private void SingleColorSplitterLightToggle(CustomColors lightColor)
    {
        SingleColorSplitterLight.color = CustomColorsUtility.CustomColorToUnityColor(lightColor); 
        
        if (true)
        {
            if (hasSplit)
            {
                if (SingleColorSplitterLight.intensity < 2)
                {
                    SingleColorSplitterLight.intensity += .01f;
                }
            }
            else
            {
                if (SingleColorSplitterLight.intensity > 0)
                {
                    SingleColorSplitterLight.intensity -= 0.01f;
                } 
            }
        }
    }
}
