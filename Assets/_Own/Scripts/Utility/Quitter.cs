using UnityEngine;
using System.Collections;

public class Quitter : MonoBehaviour
{
    public void QuitToDesktop() 
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
