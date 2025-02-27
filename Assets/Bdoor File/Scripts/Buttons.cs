using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public GameObject pauseButton;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] LayerMask uiLayer;

    private void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public void Play()
    {
        //SceneManager.LoadSceneAsync("Game");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        SetTimeScale(0);
        EnableUIInteractions();
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
        ResumeGame();
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        ResumeGame();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResumeGame();
    }

    private void ResumeGame()
    {
        SetTimeScale(1);
        DisableUIInteractions();
    }

    private void EnableUIInteractions()
    {
        var uiElements = pauseMenu.GetComponentsInChildren<CanvasGroup>();
        foreach (var element in uiElements)
        {
            element.interactable = true;
            element.blocksRaycasts = true;
        }
    }

    private void DisableUIInteractions()
    {
        var uiElements = pauseMenu.GetComponentsInChildren<CanvasGroup>();
        foreach (var element in uiElements)
        {
            element.interactable = false;
            element.blocksRaycasts = false;
        }
    }
}