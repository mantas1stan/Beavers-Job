using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Button[] buttons;

    public void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        for (int i = 0; i < unlockedLevel; i++)
        {
            buttons[i].interactable = true;
        }
    }
    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void OpenLevel(int levelId)
    {
        string levelName = "Level" + levelId;
        SceneManager.LoadScene(levelName);
    }
    public void ResetProgress()
    {
        PlayerPrefs.SetInt("ReachedIndex", 1);
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
        Cutscene1();
    }
    public void NextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
    public void PreviousLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int previousSceneIndex = Mathf.Max(0, currentSceneIndex - 1);
        SceneManager.LoadScene(previousSceneIndex);
    }
    public void Cutscene1()
    {
        SceneManager.LoadScene("Cutscene1");
    }
    public void Cutscene2()
    {
        SceneManager.LoadScene("Cutscene2");
    }
    public void Restart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
