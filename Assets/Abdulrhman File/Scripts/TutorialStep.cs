using UnityEngine;

[System.Serializable]
public class TutorialStep
{
    [Tooltip("The UI element (RectTransform) to highlight/click.")]
    public RectTransform target;

    [TextArea]
    [Tooltip("The instruction text displayed to the user.")]
    public string instruction;

    [Tooltip("If true, a pointer click event is dispatched to the target so the underlying UI also registers the click.")]
    public bool simulateUnderlyingClick = false;

    [Tooltip("Offset for the hand icon relative to the target position. (e.g., (0, -50) to position it below the target)")]
    public Vector2 handOffset = Vector2.zero;

    [Tooltip("Rotation (in degrees) for the hand icon relative to its default orientation.")]
    public float handRotation = 0f;
}
