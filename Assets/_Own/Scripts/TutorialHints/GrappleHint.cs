using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

public class GrappleHint : TutorialHint
{
    [SerializeField] TriggerEvents transitionIn;
    [SerializeField] bool leftOrRight;

    protected override void Start()
    {
        base.Start();

        Assert.IsNotNull(transitionIn);
        transitionIn.onPlayerTriggerEnter.AddListener(OnPlayerTriggerEnter);
    }

    void OnDestroy()
    {
        transitionIn.onPlayerTriggerEnter.RemoveListener(OnPlayerTriggerEnter);
    }

    protected override bool CheckTransitionOutCondition()
    {
        return Input.GetButtonDown(leftOrRight ? "Fire1" : "Fire2");
    }

    private void OnPlayerTriggerEnter()
    {
        if (!isTransitionedIn)
        {
            transitionIn.onPlayerTriggerEnter.RemoveListener(OnPlayerTriggerEnter);
            TransitionIn();
        }
    }
}
