using EasyTransition;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;
    public Image[] lockImages;
    public TransitionSettings[] transitions;
    public float startDelay = 0.5f;

    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 2);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false; 
            lockImages[i].gameObject.SetActive(true); 
        }

        buttons[0].interactable = true;
        lockImages[0].gameObject.SetActive(false);

        buttons[1].interactable = true;
        lockImages[1].gameObject.SetActive(false);

        for (int i = 2; i < unlockedLevel; i++)
        {
            buttons[i].interactable = true;
            lockImages[i].gameObject.SetActive(false);
        }
    }

    public void OpenLevel(int levelId)
    {
        string levelName = "Level " + levelId;
        if (levelId >= 0 && levelId < transitions.Length)
        {
            TransitionManager.Instance().Transition(levelName, transitions[levelId], startDelay);
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("UnlockedLevel");
        PlayerPrefs.DeleteKey("ReachedIndex");
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CompleteLevel(int currentLevel)
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 2);

        if (currentLevel + 1 > unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            PlayerPrefs.Save();
        }
    }


}
