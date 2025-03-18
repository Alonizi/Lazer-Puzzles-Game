using UnityEngine;
using TMPro;

[System.Serializable]
public class TutorialStep {
    [Tooltip("Target UI element to highlight (the click area).")]
    public RectTransform target;
    
    [TextArea]
    [Tooltip("Instruction text that appears (will be displayed with a typewriter effect).")]
    public string instruction;
}
