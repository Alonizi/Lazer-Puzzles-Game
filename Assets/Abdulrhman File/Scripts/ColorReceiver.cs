using UnityEngine;

public class SimpleColorReceiver : MonoBehaviour
{
    [Header("Target Color")]
    [Tooltip("The color the receiver is looking for (e.g., white).")]
    public Color targetColor = Color.white;

    [Header("Hit Color")]
    [Tooltip("The color the receiver changes to when hit by the correct laser.")]
    public Color hitColor = Color.green;

    [Header("Default Color")]
    [Tooltip("The default color of the receiver.")]
    public Color defaultColor = Color.gray;

    [Header("Time to Activate")]
    [Tooltip("How long the laser must hit the receiver to activate it.")]
    public float requiredHitTime = 3.0f;

    [Header("Reset Delay")]
    [Tooltip("How long the receiver stays in the hit color before resetting to the default color.")]
    public float resetDelay = 1.0f;

    private SpriteRenderer spriteRenderer;
    private bool isHit = false;
    private float hitTimer = 0.0f;

    private void Awake()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the receiver! Please add one.");
        }

        // Set the default color
        spriteRenderer.color = defaultColor;
    }

    /// <summary>
    /// Call this method when the laser hits the receiver.
    /// </summary>
    public void LaserHitting(Color laserColor)
    {
        // Only progress if the laser's color matches the target color
        if (laserColor == targetColor)
        {
            // Increment the hit timer
            hitTimer += Time.deltaTime;

            // Check if the required hit time has been reached
            if (hitTimer >= requiredHitTime && !isHit)
            {
                // Change to the hit color
                spriteRenderer.color = hitColor;
                isHit = true;

                // Trigger win state or other effects here (optional)
                Debug.Log("Receiver activated!");

                // Reset after a delay (optional)
                Invoke(nameof(ResetColor), resetDelay);
            }
        }
        else
        {
            // Reset the timer if the laser's color is incorrect
            ResetHitTimer();
        }
    }

    /// <summary>
    /// Call this method when the laser is no longer hitting the receiver.
    /// </summary>
    public void LaserStopped()
    {
        ResetHitTimer();
    }

    private void ResetHitTimer()
    {
        hitTimer = 0.0f;
    }

    private void ResetColor()
    {
        if (isHit)
        {
            spriteRenderer.color = defaultColor;
            isHit = false;
        }
    }
}
