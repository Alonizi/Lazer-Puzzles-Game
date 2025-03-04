using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SimpleColorReceiver : MonoBehaviour
{
    [Header("Target Color")]
    [Tooltip("The color the receiver is looking for (e.g., white, purple, etc.).")]
    public Color targetColor = Color.white;

    [Header("Hit Color (When Activated)")]
    [Tooltip("The color the receiver changes to when activated.")]
    public Color activatedColor = Color.white;

    [Header("Default Color (Base Color)")]
    [Tooltip("The base color of the receiver before any laser hits it.")]
    public Color baseDefaultColor = Color.gray;

    [Header("Time to Activate")]
    [Tooltip("How long the laser must hit the receiver to activate it.")]
    public float requiredHitTime = 3.0f;

    private SpriteRenderer spriteRenderer;
    private float hitTimer = 0.0f;
    private bool isActivated = false;

    private List<Color> laserColors = new List<Color>(); // Stores all laser colors hitting the receiver
    private Color combinedColor = Color.gray; // Stores the dynamically mixed color

    [Header("Win Panel")]
    public GameObject PanelWin;

    [Header("Receivers")]
    public int receiverIndex;
    public static int totalReceivers = 0;
    public static int activatedReceivers = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the receiver! Please add one.");
        }
        spriteRenderer.color = baseDefaultColor;

        totalReceivers = FindObjectsOfType<SimpleColorReceiver>().Length; 
        activatedReceivers = 0;

    }

    /// <summary>
    /// Called when a laser hits the receiver.
    /// </summary>
    public void LaserHitting(Color laserColor)
    {
        if (!laserColors.Contains(laserColor)) 
        {
            laserColors.Add(laserColor);
        }
    }

    /// <summary>
    /// Called when lasers are no longer hitting the receiver.
    /// </summary>
    public void LaserStopped()
    {
        laserColors.Clear();
        ResetHitTimer();

        if (isActivated)
        {
            isActivated = false;
            activatedReceivers--;
        }

        if (PanelWin != null)
        {
            PanelWin.SetActive(false);
        }

    }

    private void LateUpdate()
    {
        // Combine all laser colors
        combinedColor = Color.black;
        foreach (Color c in laserColors)
        {
            combinedColor = AddColors(combinedColor, c);
        }

        if (laserColors.Count > 0 && !isActivated)
        {
            spriteRenderer.color = combinedColor; // Show the mixed color while lasers hit
        }
        else if (!isActivated)
        {
            spriteRenderer.color = baseDefaultColor; // Reset if no lasers are hitting
        }

        // Check if the combined color matches the target color
        if (ApproximatelyEqual(combinedColor, targetColor))
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= requiredHitTime && !isActivated)
            {
                ActivateReceiver();
            }
        }
        else
        {
            ResetHitTimer();
            if (isActivated)
            {
                isActivated = false;
                activatedReceivers--;
                spriteRenderer.color = combinedColor; // Reset to mixed color instead of base color
            }
        }

        laserColors.Clear(); // Reset for the next frame
    }

    private void ActivateReceiver()
    {
        isActivated = true;
        activatedReceivers++;
        spriteRenderer.color = activatedColor;
        Debug.Log("Receiver activated with color: " + combinedColor);

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        SimpleColorReceiver[] receivers = FindObjectsOfType<SimpleColorReceiver>();
        foreach (SimpleColorReceiver receiver in receivers)
        {
            if (!receiver.isActivated)
            {
                return; 
            }
        }

        if (PanelWin != null)
        {
            PanelWin.SetActive(true);
            var buttons = FindObjectOfType<Buttons>();
            if (buttons != null)
            {
                buttons.Win(); 
            }
        }

        Debug.Log("All receivers activated! You win!");

        UnlockNewLevel();
    }

    private void ResetHitTimer()
    {
        hitTimer = 0.0f;
    }

    private void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Adds two colors together using additive color mixing.
    /// </summary>
    private Color AddColors(Color a, Color b)
    {
        return new Color(
            Mathf.Clamp01(a.r + b.r),
            Mathf.Clamp01(a.g + b.g),
            Mathf.Clamp01(a.b + b.b),
            1f);
    }

    /// <summary>
    /// Checks if two colors are approximately equal.
    /// </summary>
    private bool ApproximatelyEqual(Color a, Color b, float tolerance = 0.1f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}
