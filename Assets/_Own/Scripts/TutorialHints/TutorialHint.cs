using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

#pragma warning disable 0649

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Saveable))]
public class TutorialHint : MonoBehaviour
{
    struct SaveData
    {
        public bool isTransitionedIn;
        public bool isTransitionOutConditionFulfilled;
    }

    private static TutorialHint currentlyActive;

    [SerializeField] float transitionDuration = 0.25f;
    [SerializeField] float maxScale = 2f;
    [Space]
    [SerializeField] float transitionInDelay  = 0f;
    [SerializeField] float transitionOutDelay = 0.5f;
    [SerializeField] bool hideOthersOnTransition = true;
    [SerializeField] TutorialHint[] needToBeFulfilledFirst;
    [SerializeField] bool autoActivateAsSoonAsPossible;

    private CanvasGroup canvasGroup;
    private Saveable saveable;

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
        saveable = GetComponent<Saveable>();

        SaveData saveData;
        if (saveable.GetSavedData(out saveData)) 
        {
            if (saveData.isTransitionedIn) TransitionIn();
            isTransitionOutConditionFulfilled = saveData.isTransitionOutConditionFulfilled; 
        }
    }

    protected virtual void Update()
    {
        if (needToBeFulfilledFirst.Any(hint => !hint.isTransitionOutConditionFulfilled))
        {
            return;
        }

        if (!isTransitionOutConditionFulfilled && CheckTransitionOutCondition())
        {
            isTransitionOutConditionFulfilled = true;
            if (isTransitionedIn) 
            {
                CancelInvoke();
                Invoke("TransitionOut", transitionOutDelay);
                isTransitionedIn = false;
            }
        }

        if (isTransitionOutConditionFulfilled) return;

        if (!isTransitionedIn)
        {
            if (!isTransitionInConditionFulfilled && (autoActivateAsSoonAsPossible || CheckTransitionInCondition()))
            {
                isTransitionInConditionFulfilled = true;
                CancelInvoke();
                Invoke("TransitionIn", transitionInDelay);
                isTransitionedIn = true;
            }
        }
    }

    void OnDestroy()
    {
        saveable.SaveData(new SaveData() {
            isTransitionedIn = isTransitionedIn,
            isTransitionOutConditionFulfilled = isTransitionOutConditionFulfilled
        });
    }

    protected virtual bool CheckTransitionInCondition()  { return false; }
    protected virtual bool CheckTransitionOutCondition() { return false; }

    protected virtual void OnTransitionIn()  {}
    protected virtual void OnTransitionOut() {}

    public void TransitionIn()
    {
        if (isTransitionOutConditionFulfilled) return;

        if (hideOthersOnTransition && currentlyActive != null)
        {
            currentlyActive.TransitionOut();
        }

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

        currentlyActive = this;
    }

    public void TransitionOut()
    {
        if (this == currentlyActive)
        {
            currentlyActive = null;
        }

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
