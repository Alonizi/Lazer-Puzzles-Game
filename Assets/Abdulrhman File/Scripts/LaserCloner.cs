using System.Collections.Generic;
using UnityEngine;

public class LaserCloner : MonoBehaviour
{
    [Tooltip("Prefab of the laser emitter (must have LaserScript + LineRenderer).")]
    public GameObject laserPrefab;

    [Tooltip("Distance from cloner center to spawn new beams.")]
    public float spawnOffset = 0.5f;

    [Tooltip("Cooldown time in seconds between clones to prevent spam.")]
    public float cloneCooldown = 0.5f;

    // Time when the last cloning occurred.
    private float lastCloneTime = -Mathf.Infinity;

    /// <summary>
    /// Clones an incoming laser beam (of any color) into three beams with the same color.
    /// New beams exit from the three sides not directly hit.
    /// </summary>
    /// <param name="hitPoint">World point where the laser hit the cloner.</param>
    /// <param name="incomingDirection">Direction from which the laser is coming.</param>
    /// <param name="laserColor">Color of the incoming laser.</param>
    public void CloneLaser(Vector2 hitPoint, Vector2 incomingDirection, Color laserColor)
    {
        Debug.Log("[LaserCloner] CloneLaser called. HitPoint: " + hitPoint + 
                  ", IncomingDirection: " + incomingDirection + 
                  ", LaserColor: " + laserColor);

        // Check for cooldown to prevent spamming clones.
        if (Time.time < lastCloneTime + cloneCooldown)
        {
            Debug.Log("[LaserCloner] Cloner on cooldown. Current Time: " + Time.time + 
                      ", Last Clone Time: " + lastCloneTime);
            return;
        }
        lastCloneTime = Time.time;
        Debug.Log("[LaserCloner] Cooldown passed. LastCloneTime updated to: " + lastCloneTime);

        // Convert hit point to local space.
        Vector2 localHit = transform.InverseTransformPoint(hitPoint);
        Debug.Log("[LaserCloner] Local hit position: " + localHit);

        // Determine which side was hit.
        Vector2 hitSide = (Mathf.Abs(localHit.x) > Mathf.Abs(localHit.y))
            ? (localHit.x > 0 ? Vector2.right : Vector2.left)
            : (localHit.y > 0 ? Vector2.up : Vector2.down);
        Debug.Log("[LaserCloner] Hit side determined as: " + hitSide);

        // Define the four cardinal directions.
        Vector2[] sides = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        List<Vector2> outputSides = new List<Vector2>();

        // Exclude the side that was hit.
        foreach (Vector2 side in sides)
        {
            Debug.Log("[LaserCloner] Checking side: " + side + " against hit side: " + hitSide + 
                      " (Dot: " + Vector2.Dot(side, hitSide) + ")");
            if (Vector2.Dot(side, hitSide) > 0.9f)
            {
                Debug.Log("[LaserCloner] Skipping side: " + side + " (matches hit side)");
                continue;
            }
            outputSides.Add(side);
        }

        if (outputSides.Count == 0)
        {
            Debug.LogWarning("[LaserCloner] No output sides determined. Aborting cloning.");
            return;
        }

        // Spawn a new laser emitter for each valid output side.
        foreach (Vector2 side in outputSides)
        {
            Vector2 worldDir = transform.TransformDirection(side).normalized;
            Vector2 spawnPos = (Vector2)transform.position + worldDir * spawnOffset;
            Debug.Log("[LaserCloner] Spawning cloned laser on side: " + side + 
                      " at spawn position: " + spawnPos + " with world direction: " + worldDir);

            GameObject newLaser = Instantiate(laserPrefab, spawnPos, Quaternion.identity);
            if (newLaser == null)
            {
                Debug.LogError("[LaserCloner] Failed to instantiate laserPrefab!");
                continue;
            }
            float angle = Mathf.Atan2(worldDir.y, worldDir.x) * Mathf.Rad2Deg;
            newLaser.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            Debug.Log("[LaserCloner] New laser instantiated, rotation set to " + angle + " degrees.");

            LaserScript ls = newLaser.GetComponent<LaserScript>();
            if (ls != null)
            {
                ls.SetLaserColor(laserColor);
                Debug.Log("[LaserCloner] LaserScript found on cloned laser; color set to: " + laserColor);
            }
            else
            {
                Debug.LogWarning("[LaserCloner] LaserScript component not found on cloned laser.");
            }
        }
        Debug.Log("[LaserCloner] Cloning process completed.");
    }
}
