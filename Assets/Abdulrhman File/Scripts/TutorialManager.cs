using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Steps")]
    [Tooltip("List of tutorial steps to configure in the Inspector.")]
    public List<TutorialStep> steps = new List<TutorialStep>();

    [Header("UI References")]
    [Tooltip("Full-screen image with a custom material that creates a masked (blurred) overlay.")]
    public Image blurMask;
    
    [Tooltip("Hand icon that points at the target and pulses.")]
    public Image handIcon;
    
    [Tooltip("Text panel (TextMesh Pro) for instructions.")]
    public TextMeshProUGUI instructionText;
    
    [Tooltip("Exit button (X) to cancel the tutorial.")]
    public Button exitButton;
    
    [Tooltip("Panel for exit confirmation with Yes/No buttons. Ensure it has child buttons named 'YesButton' and 'NoButton'.")]
    public GameObject exitConfirmationPanel;

    [Header("Typewriter Settings")]
    [Tooltip("Speed at which text appears (seconds per character).")]
    public float typingSpeed = 0.05f;
    
    [Tooltip("Sound to play when each character is typed.")]
    public AudioClip typingSound;
    
    [Tooltip("AudioSource used for playing the typing sound.")]
    public AudioSource audioSource;

    [Header("Mask Settings")]
    [Tooltip("Additional padding (in pixels) around the target area for the mask hole.")]
    public float maskHolePadding = 10f;

    private int currentStepIndex = 0;
    private bool waitingForClick = false;
    private Canvas canvas; // Reference to the canvas where the tutorial lives

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();

        // Set up exit button listener
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClicked);
        
        // Ensure the exit confirmation panel is hidden at start
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);
    }

    private void Start()
    {
        if(steps.Count > 0)
            StartTutorialStep(currentStepIndex);
        else
            EndTutorial();
    }

    private void StartTutorialStep(int index)
    {
        if(index >= steps.Count) {
            EndTutorial();
            return;
        }
        TutorialStep step = steps[index];

        // Reposition the hand icon on the target (if available)
        if(step.target != null && handIcon != null)
        {
            handIcon.rectTransform.position = step.target.position;
        }

        // Update the mask’s hole position and size based on the target
        UpdateMaskHole(step.target);

        // Start the typewriter effect for the instruction text
        if(instructionText != null)
        {
            instructionText.text = "";
            StopAllCoroutines(); // In case any previous typewriter coroutine is running
            StartCoroutine(TypeText(step.instruction));
        }

        waitingForClick = true;
    }

    private IEnumerator TypeText(string fullText)
    {
        instructionText.text = "";
        foreach (char letter in fullText)
        {
            instructionText.text += letter;
            if (audioSource != null && typingSound != null)
            {
                audioSource.PlayOneShot(typingSound);
            }
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // Updates the mask material to cut a hole around the target area.
    // This assumes your material has properties _HoleCenter (Vector2) and _HoleRadius (float)
    private void UpdateMaskHole(RectTransform target)
    {
        if(blurMask == null || target == null) return;
        
        // Convert target position from world space to screen space
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, target.position);
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out localPos);
        // Normalize position (0 to 1) based on the canvas size (assuming canvas pivot is at 0.5,0.5)
        Vector2 pivotAdjusted = new Vector2(localPos.x / canvasSize.x + 0.5f, localPos.y / canvasSize.y + 0.5f);

        // Set shader properties for the mask (custom material used on blurMask)
        Material mat = blurMask.material;
        if(mat != null)
        {
            mat.SetVector("_HoleCenter", pivotAdjusted);
            // Calculate a radius based on the target's size (plus padding) normalized to the canvas size.
            Vector2 targetSize = target.rect.size;
            Vector2 normalizedSize = new Vector2(targetSize.x / canvasSize.x, targetSize.y / canvasSize.y);
            float radius = (normalizedSize.magnitude * 0.5f) + (maskHolePadding / Mathf.Max(canvasSize.x, canvasSize.y));
            mat.SetFloat("_HoleRadius", radius);
        }
    }

    private void Update()
    {
        if (!waitingForClick)
            return;

        // Listen for player click
        if (Input.GetMouseButtonDown(0))
        {
            TutorialStep step = steps[currentStepIndex];
            // Only process clicks that fall inside the target's area
            if (step.target != null && RectTransformUtility.RectangleContainsScreenPoint(step.target, Input.mousePosition, canvas.worldCamera))
            {
                waitingForClick = false;
                currentStepIndex++;
                StartTutorialStep(currentStepIndex);
            }
        }
    }

    // Called when the exit button (X) is clicked.
    // It displays a confirmation dialog.
    private void OnExitButtonClicked()
    {
        if(exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(true);
            // Assumes the confirmation panel has child buttons named "YesButton" and "NoButton"
            Button yesButton = exitConfirmationPanel.transform.Find("YesButton").GetComponent<Button>();
            Button noButton = exitConfirmationPanel.transform.Find("NoButton").GetComponent<Button>();
            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() => {
                exitConfirmationPanel.SetActive(false);
                EndTutorial();
            });
            noButton.onClick.AddListener(() => {
                exitConfirmationPanel.SetActive(false);
            });
        }
    }

    // Ends the tutorial by disabling the tutorial UI.
    private void EndTutorial()
    {
        gameObject.SetActive(false);
        // Optionally, you can notify other game systems that the tutorial is finished.
    }
}
