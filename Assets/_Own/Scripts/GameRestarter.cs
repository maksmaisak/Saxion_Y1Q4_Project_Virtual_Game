using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Reloads the current scene upon a button press
public class GameRestarter : MonoBehaviour 
{
    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
