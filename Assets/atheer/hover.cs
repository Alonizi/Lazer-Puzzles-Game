using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 defaultScale;
    private Vector3 hoverScale;
    private float scaleSpeed = 5f; // Adjust speed as needed
    private bool isHovered = false;

    void Start()
    {
        defaultScale = transform.localScale;
        hoverScale = defaultScale * 1.2f; // Increase by 20%
    }

    void Update()
    {
        // Smooth transition
        transform.localScale = Vector3.Lerp(transform.localScale, isHovered ? hoverScale : defaultScale, Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}
