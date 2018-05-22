using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class TutorialHint : MonoBehaviour
{
    [SerializeField] float transitionDuration = 0.25f;
    [SerializeField] float maxScale = 2f;
    [Space]
    [SerializeField] float transitionInDelay  = 0f;
    [SerializeField] float transitionOutDelay = 0.5f;

    private CanvasGroup canvasGroup;

    public bool isTransitionedIn { get; private set; }

    [SerializeField] UnityEvent _onTransitionIn = new UnityEvent();
    public UnityEvent onTransitionIn { get { return _onTransitionIn; } }
    [SerializeField] UnityEvent _onTransitionOut = new UnityEvent();
    public UnityEvent onTransitionOut { get { return _onTransitionOut; } }

    private bool isTransitionInConditionFulfilled;
    private bool isTransitionOutConditionFulfilled;

    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected virtual void Update()
    {
        if (!isTransitionedIn)
        {
            if (!isTransitionInConditionFulfilled && CheckTransitionInCondition())
            {
                isTransitionInConditionFulfilled = true;
                Invoke("TransitionIn", transitionInDelay);
            }
        }
        else
        {
            isTransitionInConditionFulfilled = true;
            if (!isTransitionOutConditionFulfilled && CheckTransitionOutCondition())
            {
                isTransitionOutConditionFulfilled = true;
                Invoke("TransitionOut", transitionOutDelay);
            }
        }
    }

    protected virtual bool CheckTransitionInCondition()  { return false; }
    protected virtual bool CheckTransitionOutCondition() { return false; }

    protected virtual void OnTransitionIn()  {}
    protected virtual void OnTransitionOut() {}

    public void TransitionIn()
    {
        canvasGroup.DOKill();
        canvasGroup
            .DOFade(1f, transitionDuration)
            .SetEase(Ease.InOutSine);

        transform.DOKill();
        transform.localScale = Vector3.one;
        transform
            .DOScale(maxScale, transitionDuration)
            .From()
            .SetEase(Ease.OutExpo);

        isTransitionedIn = true;
        OnTransitionIn();
        onTransitionIn.Invoke();
    }

    public void TransitionOut()
    {
        canvasGroup.DOKill();
        canvasGroup
            .DOFade(0f, transitionDuration)
            .SetEase(Ease.InOutSine);

        transform.DOKill(complete: true);
        transform
            .DOScale(maxScale, transitionDuration)
            .SetEase(Ease.InExpo);

        isTransitionedIn = false;
        OnTransitionOut();
        onTransitionOut.Invoke();
    }
}
