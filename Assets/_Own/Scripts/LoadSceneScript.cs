using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

public class LoadSceneScript : MonoBehaviour
{
    public void LoadScene()
    {
        if (SceneManager.GetActiveScene().name == "tutorial_level")
        {
            SceneManager.LoadScene("MainLevel");
        }
        if (SceneManager.GetActiveScene().name == "MainLevel")
        {
            Player.Instance.GetComponent<RigidbodyFirstPersonController>().mouseLook.SetCursorLock(false);
            SceneManager.LoadScene("MainMenu");
        }
    }
}
