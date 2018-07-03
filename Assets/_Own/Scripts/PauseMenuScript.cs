using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using DG.Tweening;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float maxScale = 2f;

    private CanvasGroup canvasGroup;
    private float audioListenerInitialVolume;
    public RigidbodyFirstPersonController rbFirstPersonController;
    public static bool isPaused = false;

    void Start()
    {
        audioListenerInitialVolume = AudioListener.volume;
        canvasGroup = GetComponent<CanvasGroup>();
        rbFirstPersonController = Player.Instance.GetComponent<RigidbodyFirstPersonController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        AudioListener.volume = 0;
        rbFirstPersonController.mouseLook.SetCursorLock(false);
        Cursor.visible = false;
        OnTransitionIn();
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        AudioListener.volume = audioListenerInitialVolume;
        rbFirstPersonController.mouseLook.SetCursorLock(true);
        OnTransitionOut();
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GoToMenu(string menuScene)
    {
        rbFirstPersonController.mouseLook.SetCursorLock(false);
        Time.timeScale = 1f;
        isPaused = false;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(menuScene);
        asyncOperation.completed += op => AudioListener.volume = audioListenerInitialVolume;
    }

    protected void OnTransitionIn()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOKill();
        canvasGroup
            .DOFade(1f, transitionDuration)
            .SetEase(Ease.InOutSine)
            .SetUpdate(isIndependentUpdate: true);

        transform.DOKill();
        transform.localScale = Vector3.one;
        transform
            .DOScale(maxScale, transitionDuration)
            .From()
            .SetEase(Ease.OutExpo)
            .SetUpdate(isIndependentUpdate: true);

        //GetComponentInChildren<Button>().Select();
    }

    protected void OnTransitionOut()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOKill();
        canvasGroup
            .DOFade(0f, transitionDuration)
            .SetEase(Ease.InOutSine)
            .SetUpdate(isIndependentUpdate: true);

        transform.DOKill(complete: true);
        transform
            .DOScale(maxScale, transitionDuration)
            .SetEase(Ease.InExpo)
            .SetUpdate(isIndependentUpdate: true);
    }

}
