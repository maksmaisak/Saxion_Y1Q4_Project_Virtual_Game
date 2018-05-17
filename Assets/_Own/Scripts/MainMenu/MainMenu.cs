using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void Play(string mainLevelSceneName)
    {
        SceneManager.LoadScene(mainLevelSceneName);
    }

    public void Tutorial(string tutorialSceneName)
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
