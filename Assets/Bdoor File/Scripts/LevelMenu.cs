using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;
    public Image[] lockImages;

    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
            lockImages[i].gameObject.SetActive(true);
        }
        for (int i = 0;i < unlockedLevel; i++)
        {
            buttons[i].interactable |= true;
            lockImages[i].gameObject.SetActive(false);
        }
    }

    public void OpenLevel(int levelId)
    {
        string levelName = "Level " + levelId;
        SceneManager.LoadScene(levelName);
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("UnlockedLevel");
        PlayerPrefs.DeleteKey("ReachedIndex");
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();

        // ÅÚÇÏÉ ÊÍãíá ÇáÞÇÆãÉ áÊÍÏíË ÍÇáÉ ÇáÃÒÑÇÑ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
