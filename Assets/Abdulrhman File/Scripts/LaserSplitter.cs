using System.Collections.Generic;
using UnityEngine;

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

    // Flag to ensure we only split once per hit.
    private bool hasSplit = false;

    /// <summary>
    /// Splits an incoming white laser into three beams: red, green, blue.
    /// New beams exit from the three sides not directly hit.
    /// </summary>
    /// <param name="hitPoint">World point where the laser hit the splitter.</param>
    /// <param name="incomingDirection">Direction from which the laser is coming.</param>
    public void SplitLaser(Vector2 hitPoint, Vector2 incomingDirection)
    {
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

        // Define the three primary RGB colors.
        Color[] splitColors = new Color[] { Color.red, Color.green, Color.blue };

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
                Color c = splitColors[i % splitColors.Length];
                laserScript.SetLaserColor(c);
            }
            else
            {
                Debug.LogWarning("LaserScript component not found on the instantiated laser prefab.");
            }
        }
    }
}
