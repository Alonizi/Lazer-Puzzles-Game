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
    public CustomColors targetColor;
    public CustomColors targetColor2;
    public CustomColors MixedColor;

    [Header("Use Two Target Colors")]
    [Tooltip("If true, the receiver will look for two colors.")]
    public bool two = false;

    [Header("Time to Activate")]
    [Tooltip("How long the laser must hit the receiver to activate it.")]
    public float requiredHitTime = 3.0f;

    private SpriteRenderer spriteRenderer;
    private float hitTimer = 0.0f;
    private bool isActivated = false;

    private List<CustomColors> laserColors = new List<CustomColors>(); // Stores all laser colors hitting the receiver

    [Header("Win Panel")]
    public GameObject PanelWin;

    [Header("Symbols")] 
    [SerializeField]private Transform HazardSymbol;
    [SerializeField] private Transform TriangleSymbol; 
    
    

    [Header("Receivers")]
    public int receiverIndex;
    public static int totalReceivers = 0;
    public static int activatedReceivers = 0;

    [Header("Lights")]
    public Light2D[] DiagonalLights;
    public Light2D[] CenterLight;
    public Light2D TriangleLight;

    private Rigidbody2D[] ReactorRigids;
    private float ReactorSpinSpeed = 0f;
    private float InitialCenterLightIntensity;
    private float InitialDiagonalLightIntensity;
    private float LightTimer = 0f;
    private float LightMax = 1f;
    private float LightMin = 0f;
    private float LightInterpolate = 0f;
    private bool LightToggle = false;
    
    [Header("SFX")]
    [SerializeField] private AudioSource Reactor_ON;
    [SerializeField] private AudioSource Reactor_OFF;
    [SerializeField] private AudioSource Reactor_Running;
    [SerializeField] private AudioSource Hazard_Beep;
    
    [Header("SFX")]
    [SerializeField] private ParticleSystem Pulse_VFX;

    

    private int OnCounter = 0;
    private int OffCounter = 0;
    
    
    [Header("Camera Shake")]
    [Tooltip("Optional: Reference to the CameraShake component. If left empty, the script will try to find one on the main camera.")]
    public CameraShake cameraShake;

    public float F =.5f;

    private void Awake()
    {
        //InitialCenterLightIntensity = CenterLight.intensity;
        InitialDiagonalLightIntensity = DiagonalLights[0].intensity;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the receiver! Please add one.");
        }
        
        // Updated API call using FindObjectsByType with no sorting for speed.
        totalReceivers = UnityEngine.Object.FindObjectsByType<SimpleColorReceiver>(FindObjectsSortMode.None).Length;
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
        bool wrongColorReached = false; 

        foreach (CustomColors c in laserColors)
        {
            if (c == targetColor)
            {
                color1Reached = true;
            }
            if (two && c == targetColor2)
            {
                color2Reached = true;
            }
        }

        if (laserColors.Count == 1)
        {
            wrongColorReached = !color1Reached && !color2Reached;
        }
        else if (laserColors.Count > 1)
        {
            wrongColorReached =  !color1Reached || !color2Reached;
        }
        
        bool onlyTargetColors = (two && laserColors.Count == 2 && color1Reached && color2Reached) ||
                                  (!two && laserColors.Count == 1 && color1Reached);

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
                }
            }
        }
        
        if (two)
        {
            PlayAudio(color1Reached && color2Reached,wrongColorReached);
            RotateBase(color1Reached, color2Reached);
            ActivateLightEffects(color1Reached, color2Reached , wrongColorReached);
        }
        else
        {
            PlayAudio(color1Reached,wrongColorReached);
            RotateBase(color1Reached, color1Reached);
            ActivateLightEffects(color1Reached, color1Reached, wrongColorReached);
        }
        laserColors.Clear(); // Reset for the next frame
    }

    private void ActivateReceiver()
    {
        isActivated = true;
        activatedReceivers++;
        // Removed camera shake call from here; it now triggers only after the win panel appears.
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        // Updated to use the new FindObjectsByType API.
        SimpleColorReceiver[] receivers = UnityEngine.Object.FindObjectsByType<SimpleColorReceiver>(FindObjectsSortMode.None);
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
            // Updated to use the new FindAnyObjectByType API.
            var buttons = UnityEngine.Object.FindAnyObjectByType<Buttons>();
            if (buttons != null)
            {
                buttons.Win();
            }
        }

        // Trigger camera shake only after the win panel appears.
        if (cameraShake != null)
        {
            cameraShake.TriggerShake();
        }
        else if (Camera.main != null)
        {
            CameraShake shake = Camera.main.GetComponent<CameraShake>();
            if (shake != null)
                shake.TriggerShake();
        }

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

    private void ActivateLightEffects(bool color1, bool color2 , bool wrongColor)
    {
        
        HazradLightEffect(wrongColor);
        DiagonalLightsEffect(color1, color2,wrongColor);
        
        
            // if (color1 && color2)
            // {
            //     //color = CustomColorsUtility.CustomColorToDefaultUnityColor(MixedColor);
            //     if (GetComponent<Light>().intensity < InitialDiagonalLightIntensity)
            //     {
            //         GetComponent<Light>().intensity += 0.01f;
            //     }
            // }
            // else if (color1 || color2)
            // {
            //     GetComponent<Light>().color = CustomColorsUtility.CustomColorToDefaultUnityColor(laserColors[0]);
            //     LightStrobe(GetComponent<Light>(), freq);
            // }
            // else
            // {
            //     if (GetComponent<Light>().intensity > 0)
            //     {
            //         GetComponent<Light>().intensity -= 0.01f;
            //     }
            // }
    }

    private void RotateBase(bool color1, bool color2)
    {
        var multiplier = (color1 && color2 ? 4 : 1);
        
        if (color1 || color2)
        {
            if (ReactorSpinSpeed < 40f * multiplier)
            {
                ReactorSpinSpeed += 0.35f;
            }
            else if (ReactorSpinSpeed > 40f * multiplier)
            {
                ReactorSpinSpeed -= 0.25f;

            }
        }
        else 
        {
            if (ReactorSpinSpeed > 0)
            {
                ReactorSpinSpeed -= 0.1f;
            }
        }
        ReactorRigids[1].rotation -= ReactorSpinSpeed * Time.deltaTime;
        ReactorRigids[0].rotation += ReactorSpinSpeed * Time.deltaTime;
    }

    private void LightStrobe(Light2D light, float freq)
    {
        LightTimer += Time.deltaTime;
        if (LightTimer > freq)
        {
            light.intensity = Random.Range(0f, 2f);
            LightTimer = 0f;
        }
    }

    private void PlayAudio(bool on , bool wrongColor)
    {
        if (on && OnCounter < 1)
        {
            Reactor_Running.Stop();
            Reactor_OFF.Stop();
            //Reactor_ON.Play();
            OnCounter++;
            OffCounter = 0;
        }
        if (on && !Reactor_ON.isPlaying && !Reactor_Running.isPlaying)
        {
            Reactor_Running.Play();
        }
        if (!on && OffCounter < 1)
        {
            OffCounter++;
            OnCounter = 0;
            Reactor_ON.Stop();
            Reactor_Running.Stop();
            Reactor_OFF.Play();
        }

        if (wrongColor && !Hazard_Beep.isPlaying)
        {
            Hazard_Beep.Play();
        }
    }

    private void LightPulsing(Light2D light, float f )
    {
        
        LightInterpolate += F * Time.deltaTime;
        light.intensity = Mathf.Lerp(LightMin, LightMax, LightInterpolate);

        if (LightInterpolate > 1f)
        {
            float intensity = LightMax;
            LightMax = LightMin;
            LightMin = intensity;
            LightInterpolate = 0f; 
        }
    }

    private void HazradLightEffect(bool wrongColor)
    {
        if (wrongColor) 
        {
            TriangleSymbol.gameObject.SetActive(false);
            LightTimer += Time.deltaTime;
            if (LightTimer > 0.24f)
            {
                HazardSymbol.gameObject.SetActive(true);
                LightToggle = !LightToggle;
                LightTimer = 0;
                CenterLight[0].intensity = 5f * (LightToggle ? 1 : 0);
                CenterLight[1].intensity = 5f * (LightToggle ? 1 : 0);
                CenterLight[2].intensity = 5f * (LightToggle ? 1 : 0);
            }
        }
        else
        {
            HazardSymbol.gameObject.SetActive(false);
        }
    }

    private void DiagonalLightsEffect(bool color1, bool color2 ,bool wrongColor)
    {
        if (color1 && color2)
        {
            if (!Pulse_VFX.isPlaying)
            {
                Pulse_VFX.Play();
            }
            TriangleSymbol.gameObject.SetActive(true);
            if (TriangleLight.intensity < 3)
            {
                TriangleLight.intensity += 0.005f;
            }
            
        }
        else if (two && (color1 || color2) && !wrongColor)
        {
            Pulse_VFX.Stop();
            TriangleSymbol.gameObject.SetActive(true);
            LightStrobe(TriangleLight, Random.Range(0,1.5f));
        }
        else
        {
            Pulse_VFX.Stop();
            if (TriangleLight.intensity > 0)
            {
                TriangleLight.intensity -= 0.0025f;
            }

            if (TriangleLight.intensity <= 0)
            {
                TriangleSymbol.gameObject.SetActive(false);
            }
        }
        
        
        
        foreach (var light in DiagonalLights)
        {
            if (color1 && color2)
            { 
                LightPulsing(light,0.075f);
            }
            else
            {
                if (light.intensity > 0)
                {
                    light.intensity -= 0.015f;
                }
            }
        }
    }
    
}
