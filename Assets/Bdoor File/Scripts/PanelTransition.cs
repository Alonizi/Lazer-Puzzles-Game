using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelTransition : MonoBehaviour
{
    public Button[] openButtons;
    public GameObject[] panels;
    public Button[] closeButtons;
    public float fadeDuration = 0.5f;

    private CanvasGroup[] panelCanvasGroups;

    private void Start()
    {
        if (openButtons.Length != panels.Length || closeButtons.Length != panels.Length)
            return;

        panelCanvasGroups = new CanvasGroup[panels.Length];
        for (int i = 0; i < panels.Length; i++)
        {
            panelCanvasGroups[i] = panels[i].AddComponent<CanvasGroup>();
            panelCanvasGroups[i].alpha = 0;
            panels[i].SetActive(false);
        }

        for (int i = 0; i < openButtons.Length; i++)
        {
            int index = i;
            openButtons[i].onClick.AddListener(() => StartPanelTransition(index));
        }

        for (int i = 0; i < closeButtons.Length; i++)
        {
            int index = i;
            closeButtons[i].onClick.AddListener(() => ClosePanelWithTransition(index));
        }
    }

    private void StartPanelTransition(int targetIndex)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == targetIndex)
                StartCoroutine(FadeIn(panelCanvasGroups[i]));
            else
                StartCoroutine(FadeOut(panelCanvasGroups[i], null));
        }
    }

    private void ClosePanelWithTransition(int panelIndex)
    {
        if (panelIndex >= 0 && panelIndex < panels.Length)
        {
            StartCoroutine(FadeOut(panelCanvasGroups[panelIndex], () =>
            {
                panels[panelIndex].SetActive(false);
            }));
        }
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        panels[System.Array.IndexOf(panelCanvasGroups, canvasGroup)].SetActive(true);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup, System.Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        onComplete?.Invoke();
    }

}
