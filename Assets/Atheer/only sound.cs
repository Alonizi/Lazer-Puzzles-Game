using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundEffects : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private Button button; // Reference to the Button component

    [Tooltip("AudioSource for playing the sound effects.")]
    public AudioSource audioSource;

    [Tooltip("Sound to play when the pointer hovers over an active button.")]
    public AudioClip hoverSound;

    [Tooltip("Sound to play when an active button is clicked.")]
    public AudioClip clickSound;

    [Tooltip("Sound to play when a disabled (locked) button is clicked.")]
    public AudioClip lockedButtonSound;

    void Awake()
    {
        // Cache the Button component on this GameObject.
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Only play the hover sound if the button is interactable.
        if (button != null && button.interactable)
        {
            if (audioSource != null && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button != null)
        {
            if (button.interactable)
            {
                // Active button: play click sound.
                if (audioSource != null && clickSound != null)
                {
                    audioSource.PlayOneShot(clickSound);
                }
            }
            else
            {
                // Disabled button: play locked button sound only.
                if (audioSource != null && lockedButtonSound != null)
                {
                    audioSource.PlayOneShot(lockedButtonSound);
                }
                // Consume the event so that no active button click sound is triggered.
                eventData.Use();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // No sound needed on pointer exit.
    }
}
