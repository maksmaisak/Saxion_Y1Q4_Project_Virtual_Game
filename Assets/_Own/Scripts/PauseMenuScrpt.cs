using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine;

public class PauseMenuScrpt : MonoBehaviour {

    [SerializeField] private float transitionDuration = 1;
    [SerializeField] private float maxScale = 2;
    private CanvasGroup canvasGroup;

    public static bool isPaused = false;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    protected void OnTransitionIn()
    {
        canvasGroup.interactable = true;
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

    public void Pause()
    {
        OnTransitionIn();
        isPaused = true;
        Time.timeScale = 0;
    }
    public void Resume()
    {
        OnTransitionOut();
        isPaused = false;
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GoToMenu(string menuScene)
    {
        SceneManager.LoadScene(menuScene);
    }
}
