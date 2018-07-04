using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

/// Makes sure the management scene is loaded alongside this one on startup.
public class EnsureManagementScene : MonoBehaviour
{
    void Awake()
    {
        transform.parent = null;
        transform.SetAsFirstSibling();

        if (gameObject.scene.name == SceneNames.managementName) return;

        Scene managementScene = SceneManager.GetSceneByName(SceneNames.managementName);
        if (managementScene.IsValid()) return;

        StartCoroutine(LoadManagementScene());
    }

    private IEnumerator LoadManagementScene()
    {
        var rootGameObjectsActivationPattern = DeactivateRootGameObjects();

        SceneManager.LoadScene(SceneNames.managementName, LoadSceneMode.Additive);
        yield return null;

        Assert.IsTrue(SceneManager.sceneCount == rootGameObjectsActivationPattern.Length + 1);
        ActivateRootGameObjects(rootGameObjectsActivationPattern);
    }

    private bool[][] DeactivateRootGameObjects()
    {
        // isActivated[i][j] is whether or not the jth root GameObject in ith scene is activeSelf.
        var isActivated = new bool[SceneManager.sceneCount][];

        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            GameObject[] roots = loadedScene.GetRootGameObjects();
            isActivated[i] = new bool[roots.Length];

            for (int j = 0; j < roots.Length; ++j)
            {
                isActivated[i][j] = roots[j].activeSelf;

                if (roots[j] != gameObject)
                {
                    roots[j].SetActive(false);
                }
            }
        }

        return isActivated;
    }

    private void ActivateRootGameObjects(bool[][] rootGameObjectsActivationPattern)
    {
        for (int i = 0; i < rootGameObjectsActivationPattern.Length; ++i)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            GameObject[] roots = loadedScene.GetRootGameObjects();

            for (int j = 0; j < roots.Length; ++j)
            {
                roots[j].SetActive(rootGameObjectsActivationPattern[i][j]);
            }
        }
    }
}
