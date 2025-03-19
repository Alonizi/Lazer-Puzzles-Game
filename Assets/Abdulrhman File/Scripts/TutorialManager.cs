using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TutorialManager : MonoBehaviour
{
    [Header("General Settings")]
    [Tooltip("Delay before the tutorial starts (in seconds).")]
    public float startDelay = 0f;

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

    [Header("Animation Settings")]
    [Tooltip("Duration for the mask hole animations (in seconds).")]
    public float animationDuration = 0.5f;

    private int currentStepIndex = 0;
    private bool waitingForClick = false;
    private Canvas canvas; // Reference to the canvas where the tutorial lives

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();

        // Set the mask hole to 0 when the scene loads
        if (blurMask != null && blurMask.material != null)
        {
            blurMask.material.SetFloat("_HoleRadius", 0f);
        }

        // Set up exit button listener
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClicked);
        
        // Hide the exit confirmation panel at start
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);
    }

    private void Start()
    {
        if (startDelay > 0)
            StartCoroutine(StartAfterDelay());
        else
            BeginTutorial();
    }

    private IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        BeginTutorial();
    }

    private void BeginTutorial()
    {
        if (steps.Count > 0)
        {
            TutorialStep firstStep = steps[0];
            if (firstStep.target != null)
            {
                // Update the mask center based on the target
                UpdateMaskHole(firstStep.target);
                float targetRadius = GetHoleRadius(firstStep.target);
                // Animate the hole radius from 0 to the computed size and then start step 0
                StartCoroutine(BeginTutorialAfterAnimation(targetRadius));
            }
            else
            {
                StartTutorialStep(0);
            }
        }
        else
        {
            EndTutorial();
        }
    }

    private IEnumerator BeginTutorialAfterAnimation(float targetRadius)
    {
        yield return StartCoroutine(AnimateHoleRadius(0, targetRadius, animationDuration));
        StartTutorialStep(0);
    }

    private void StartTutorialStep(int index)
    {
        if (index >= steps.Count)
        {
            EndTutorial();
            return;
        }
        TutorialStep step = steps[index];

        // Position the hand icon on the target, applying the offset and rotation
        if (step.target != null && handIcon != null)
        {
            handIcon.rectTransform.position = step.target.position + (Vector3)step.handOffset;
            handIcon.rectTransform.rotation = Quaternion.Euler(0f, 0f, step.handRotation);
        }

        // Update the mask's hole based on the target (calculation remains unchanged)
        UpdateMaskHole(step.target);

        // Begin the typewriter effect for the instruction text
        if (instructionText != null)
        {
            instructionText.text = "";
            StopAllCoroutines();
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
    private void UpdateMaskHole(RectTransform target)
    {
        if (blurMask == null || target == null) return;
        
        // Convert target position to screen space
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, target.position);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.sizeDelta;
        
        // Convert to local coordinates
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            canvas.worldCamera,
            out Vector2 localPos
        );

        // Normalize position (assuming canvas pivot is at 0.5, 0.5)
        Vector2 pivotAdjusted = new Vector2(localPos.x / canvasSize.x + 0.5f, localPos.y / canvasSize.y + 0.5f);

        Material mat = blurMask.material;
        if (mat != null)
        {
            mat.SetVector("_HoleCenter", pivotAdjusted);
            // Calculate and immediately set the correct radius (used during steps)
            float radius = GetHoleRadius(target);
            mat.SetFloat("_HoleRadius", radius);
        }
    }

    // Returns the calculated hole radius based on the target's size and padding.
    private float GetHoleRadius(RectTransform target)
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.sizeDelta;
        Vector2 targetSize = target.rect.size;
        Vector2 normalizedSize = new Vector2(targetSize.x / canvasSize.x, targetSize.y / canvasSize.y);
        float radius = (normalizedSize.magnitude * 0.5f) + (maskHolePadding / Mathf.Max(canvasSize.x, canvasSize.y));
        return radius;
    }

    // Coroutine to animate the _HoleRadius property of the blur mask.
    private IEnumerator AnimateHoleRadius(float startRadius, float endRadius, float duration)
    {
        Material mat = blurMask.material;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float newRadius = Mathf.Lerp(startRadius, endRadius, elapsed / duration);
            mat.SetFloat("_HoleRadius", newRadius);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_HoleRadius", endRadius);
    }

    private void Update()
    {
        if (!waitingForClick)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            TutorialStep step = steps[currentStepIndex];
            if (step.target != null &&
                RectTransformUtility.RectangleContainsScreenPoint(step.target, Input.mousePosition, canvas.worldCamera))
            {
                if (step.simulateUnderlyingClick)
                {
                    SimulateClickOnTarget(step.target.gameObject);
                }
                waitingForClick = false;
                currentStepIndex++;
                StartTutorialStep(currentStepIndex);
            }
        }
    }

    /// <summary>
    /// Sends pointerDown, pointerUp, and pointerClick events to the specified GameObject.
    /// </summary>
    private void SimulateClickOnTarget(GameObject targetObject)
    {
        if (EventSystem.current == null) return;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        ExecuteEvents.Execute(targetObject, pointerData, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute(targetObject, pointerData, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(targetObject, pointerData, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(targetObject, pointerData, ExecuteEvents.pointerClickHandler);
    }

    private void OnExitButtonClicked()
    {
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(true);
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

    private void EndTutorial()
    {
        // Animate the hole radius back to 0 before disabling the tutorial.
        StartCoroutine(EndTutorialAnimation());
    }

    private IEnumerator EndTutorialAnimation()
    {
        Material mat = blurMask.material;
        float currentRadius = mat.GetFloat("_HoleRadius");
        yield return StartCoroutine(AnimateHoleRadius(currentRadius, 0, animationDuration));
        gameObject.SetActive(false);
        // Optionally, notify other systems that the tutorial has finished.
    }
}
