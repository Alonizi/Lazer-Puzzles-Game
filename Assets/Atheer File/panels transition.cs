using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneAndPanelTransition : MonoBehaviour
{
    [Header("Panels")]
    public CanvasGroup settingsPanel;  // Settings Panel CanvasGroup
    public CanvasGroup creditsPanel;   // Credits Panel CanvasGroup
    public CanvasGroup levelsPanel;    // Levels Panel CanvasGroup
    public float fadeDuration = 0.5f;  // Duration of fade animations

    [Header("Scene Transition")]
    public CanvasGroup fadeCanvasGroup; // Full-screen CanvasGroup for scene fade
    public string mainMenuSceneName = "MainMenu"; // Name of the Main Menu scene

    private CanvasGroup currentPanel;   // Track the currently visible panel

    private void Start()
    {
        // Set up panels and fade canvas group
        InitializePanels();
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f; // Ensure the fade canvas is invisible at the start
        }
    }

    private void InitializePanels()
    {
        currentPanel = null; // Start with no panel active
        SetPanelVisibility(settingsPanel, false);
        SetPanelVisibility(creditsPanel, false);
        SetPanelVisibility(levelsPanel, false);
    }

    private void SetPanelVisibility(CanvasGroup panel, bool isVisible)
    {
        if (panel != null)
        {
            panel.alpha = isVisible ? 1 : 0;
            panel.interactable = isVisible;
            panel.blocksRaycasts = isVisible;
            panel.gameObject.SetActive(isVisible);
        }
    }

    public void ShowPanel(CanvasGroup panelToShow)
    {
        if (currentPanel != null && currentPanel != panelToShow)
        {
            StartCoroutine(SwitchPanels(currentPanel, panelToShow));
        }
        else
        {
            StartCoroutine(FadeIn(panelToShow));
        }
        currentPanel = panelToShow;
    }

    public void HidePanel(CanvasGroup panelToHide)
    {
        if (currentPanel == panelToHide)
        {
            StartCoroutine(FadeOut(panelToHide));
            currentPanel = null;
        }
    }

    private IEnumerator SwitchPanels(CanvasGroup fromPanel, CanvasGroup toPanel)
    {
        // Fade out current panel
        yield return StartCoroutine(FadeOut(fromPanel));
        // Fade in the new panel
        yield return StartCoroutine(FadeIn(toPanel));
    }

    private IEnumerator FadeIn(CanvasGroup panel)
    {
        panel.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panel.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    private IEnumerator FadeOut(CanvasGroup panel)
    {
        panel.interactable = false;
        panel.blocksRaycasts = false;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panel.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        panel.gameObject.SetActive(false);
    }

    public void LoadMainMenu()
    {
        StartCoroutine(TransitionToScene(mainMenuSceneName));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        // Fade the screen out
        if (fadeCanvasGroup != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                yield return null;
            }
        }

        // Load the Main Menu scene
        SceneManager.LoadScene(sceneName);
    }
}