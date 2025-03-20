using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 defaultScale;
    private Vector3 hoverScale;
    private float scaleSpeed = 5f; // Adjust speed as needed
    private bool isHovered = false;
    private Button button; // Reference to the Button component

    void Start()
    {
        defaultScale = transform.localScale;
        hoverScale = defaultScale * 1.2f; // Scale up by 20%
        button = GetComponent<Button>(); // Get the Button component
    }

    void Update()
    {
        // Smooth transition only if the button is interactable
        if (button != null && button.interactable)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, isHovered ? hoverScale : defaultScale, Time.deltaTime * scaleSpeed);
        }
        else
        {
            // Reset scale if button becomes non-interactable
            transform.localScale = defaultScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Only trigger hover effect if button is interactable
        if (button != null && button.interactable)
        {
            isHovered = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}
