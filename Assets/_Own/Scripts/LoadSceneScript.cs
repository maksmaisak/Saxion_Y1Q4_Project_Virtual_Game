using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class LoadSceneScript : MonoBehaviour
{
    // Sort of meh, but works. Would be better if we could specify the next level for each scene in the editor.
    private static readonly Dictionary<string, string> sceneNameToNextSceneName = new Dictionary<string, string>()
    {
        {SceneNames.tutorialName, SceneNames.mainLevelName},
        {SceneNames.mainLevelName, SceneNames.mainMenuName}
    };

    public void LoadScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        Assert.IsTrue(activeScene.IsValid());

        string nextSceneName;
        bool success = sceneNameToNextSceneName.TryGetValue(activeScene.name, out nextSceneName);
        Assert.IsTrue(success, "No next scene found for " + activeScene.name);

        SceneManager.LoadScene(nextSceneName);
    }
}
