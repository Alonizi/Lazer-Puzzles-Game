using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class SimpleColorReceiver : MonoBehaviour
{
    [Header("Target Color")]
    [Tooltip("The color the receiver is looking for (e.g., white, purple, etc.).")]
    public CustomColors targetColor ;
    public CustomColors targetColor2 ;

    [Header("Use Two Target Colors")]
    [Tooltip("If true, the receiver will look for two colors.")]
    public bool two = false;

    // [Header("Hit Color (When Activated)")]
    // [Tooltip("The color the receiver changes to when activated.")]
    // public Color activatedColor = Color.white;

    // [Header("Default Color (Base Color)")]
    // [Tooltip("The base color of the receiver before any laser hits it.")]
    // public Color baseDefaultColor = Color.gray;

    [Header("Time to Activate")]
    [Tooltip("How long the laser must hit the receiver to activate it.")]
    public float requiredHitTime = 3.0f;

    private SpriteRenderer spriteRenderer;
    private float hitTimer = 0.0f;
    private bool isActivated = false;

    private List<CustomColors> laserColors = new List<CustomColors>(); // Stores all laser colors hitting the receiver
    // private Color combinedColor = Color.gray; // Stores the dynamically mixed color

    [Header("Win Panel")]
    public GameObject PanelWin;

    [Header("Receivers")]
    public int receiverIndex;
    public static int totalReceivers = 0;
    public static int activatedReceivers = 0;

    [Header("effects")] 
    public Light2D[] DiagonalLights;
    public Light2D CenterLight;

    private Rigidbody2D[] ReactorRigids;

    private float ReactorSpinSpeed = 0f;
    private float InitialCenterLightIntensity;
    private float InitialDiagonalLightIntensity;
    private float LightTimer = 0;
    
    [SerializeField] private AudioSource Reactor_ON;
    [SerializeField] private AudioSource Reactor_OFF;
    [SerializeField] private AudioSource Reactor_Running;
    private int OnCounter= 0;
    private int OffCounter =0;
    
    private void Awake()
    {
        //Reactor_Running.loop = true; 
        InitialCenterLightIntensity = CenterLight.intensity;
        InitialDiagonalLightIntensity = DiagonalLights[0].intensity;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the receiver! Please add one.");
        }
        // spriteRenderer.color = baseDefaultColor;

        totalReceivers = FindObjectsOfType<SimpleColorReceiver>().Length; 
        activatedReceivers = 0;

        ReactorRigids = transform.GetComponentsInChildren<Rigidbody2D>();
    }

    /// <summary>
    /// Called when a laser hits the receiver.
    /// </summary>
    public void LaserHitting(CustomColors laserColor)
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
        bool color1Reached = false;
        bool color2Reached = false;

        foreach (CustomColors c in laserColors)
        {
            if (c == targetColor)
            {
                color1Reached = true;
            }
            if (two && c==targetColor2)
            {
                color2Reached = true;
            }
        }

        bool onlyTargetColors = (two && laserColors.Count == 2 && color1Reached && color2Reached) || (!two && laserColors.Count == 1 && color1Reached);

        if (two)
        {
            if (onlyTargetColors)
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
                    //spriteRenderer.color = combinedColor; // Reset to mixed color instead of base color
                }
            }
        }
        else
        {
            if (onlyTargetColors)
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
                    //spriteRenderer.color = combinedColor; // Reset to mixed color instead of base color
                }
            }
        }
        
        if (two)
        {
            PlayAudio(color1Reached && color2Reached);
            RotateBase(color1Reached, color2Reached);
            ActivateEffects(color1Reached, color2Reached);
        }
        else
        {
            PlayAudio(color1Reached);
            RotateBase(color1Reached, color1Reached);
            ActivateEffects(color1Reached, color1Reached);
        }
        laserColors.Clear(); // Reset for the next frame
    }

    private void ActivateReceiver()
    {
        isActivated = true;
        activatedReceivers++;
        //spriteRenderer.color = activatedColor;
        Debug.Log("Receiver activated with colors");

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

    private void ActivateEffects(bool color1 , bool color2)
    {
        //CenterLight.enabled = color1 && color2 ;
        CenterLight.enabled = true;
        if (color1 && color2)
        {
            if (CenterLight.intensity < InitialCenterLightIntensity)
            {
                CenterLight.intensity += .01f;
            }
        }
        else
        {
            if (CenterLight.intensity > 0)
            {
                CenterLight.intensity -= .015f;
            }
        }

        var freq =  Random.Range(.25f, 1f);
        foreach (var light in DiagonalLights)
        {
            light.enabled = true;
            //light.enabled = color1 || color2 ;
            if (color1 && color2)
            {
                if (light.intensity < InitialDiagonalLightIntensity)
                {
                    light.intensity += .01f;
                }
                
            }
            else if (color1 || color2)
            {
                lightstrobe(light,freq);
            }
            else
            {
                if (light.intensity > 0)
                {
                    light.intensity -= .01f;
                }
            }
        }
    }

    private void RotateBase( bool color1 , bool color2)
    { 
        //ReactorRigids[0].rotation += 60f * Time.deltaTime * (color1 || color2 ? 1:0);
        // ReactorRigids[1].rotation -= ReactorSpinSpeed * Time.deltaTime * (color1 && color2 ? 1:0);
       
        if( color1 && color2)
        {
            if (ReactorSpinSpeed < 250f)
            {
                ReactorSpinSpeed += .5f;
            }
        }
        else
        {
            if (ReactorSpinSpeed > 0)
            {
                ReactorSpinSpeed -= 0.5f;
            }
        }
        ReactorRigids[1].rotation -= ReactorSpinSpeed * Time.deltaTime;
    }

    private void lightstrobe(Light2D light , float freq)
    {
        LightTimer += Time.deltaTime;
        if (LightTimer > freq)
        {
            light.intensity = Random.Range(0f, 3f);
            LightTimer = 0; 
        }
    }

    private void PlayAudio(bool on )
    {
        if (on && OnCounter <1)
        {
            Reactor_Running.Stop();
            Reactor_OFF.Stop();
            Reactor_ON.Play();
            OnCounter++;
            OffCounter = 0; 
        }
        if (on && !Reactor_ON.isPlaying && !Reactor_Running.isPlaying)
        {
            Reactor_Running.Play();
        }
        if (!on && OffCounter<1 )
        {
            OffCounter++;
            OnCounter = 0; 
            Reactor_ON.Stop();
            Reactor_Running.Stop();
            Reactor_OFF.Play();
        }


    }
    
}