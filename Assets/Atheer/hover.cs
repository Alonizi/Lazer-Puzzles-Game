using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private Vector3 defaultScale;
    private Vector3 hoverScale;
    private float scaleSpeed = 5f; // Adjust speed as needed
    private bool isHovered = false;
    private Button button; // Reference to Button component

    public AudioSource audioSource;           // Shared AudioSource for sounds
    public AudioClip hoverSound;              // Sound for hover on active buttons
    public AudioClip clickSound;              // Sound for clicking active buttons
    public AudioClip lockedButtonSound;       // Sound for clicking inactive (locked) buttons

    // Shake parameters for inactive buttons
    public float shakeDuration = 0.2f;        // Duration of the shake in seconds
    public float shakeMagnitude = 10f;        // Magnitude of the shake (in pixels, for example)

    void Start()
    {
        defaultScale = transform.localScale;
        hoverScale = defaultScale * 1.2f; // Increase by 20%
        button = GetComponent<Button>();  // Get the Button component on this GameObject
    }

    void Update()
    {
        if (button != null && button.interactable)
        {
            // Smooth transition between default and hover scales only if the button is interactable
            transform.localScale = Vector3.Lerp(transform.localScale, isHovered ? hoverScale : defaultScale, Time.deltaTime * scaleSpeed);
        }
        else
        {
            // Ensure scale resets when the button is inactive
            transform.localScale = defaultScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            isHovered = true;

            // Play hover sound only if the button is active
            if (audioSource != null && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button != null)
        {
            if (button.interactable)
            {
                // Active button: play click sound
                if (audioSource != null && clickSound != null)
                {
                    audioSource.PlayOneShot(clickSound);
                }
            }
            else
            {
                // Inactive button: play locked button sound then shake the button
                if (audioSource != null && lockedButtonSound != null)
                {
                    audioSource.PlayOneShot(lockedButtonSound);
                }
                StartCoroutine(Shake());
            }
        }
    }

    // Coroutine for a sudden shake effect
    IEnumerator Shake()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Random offset in x and y directions
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Reset to original position after shake
        transform.localPosition = originalPos;
    }
}
