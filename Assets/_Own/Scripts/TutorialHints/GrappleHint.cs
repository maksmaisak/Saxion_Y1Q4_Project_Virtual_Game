using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

public class GrappleHint : TutorialHint
{
    [SerializeField] TriggerEvents transitionIn;
    [SerializeField] bool trackRightGrapple;

    protected override void Start()
    {
        base.Start();

        Assert.IsNotNull(transitionIn);
        transitionIn.onPlayerTriggerEnter.AddListener(OnPlayerTriggerEnter);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        transitionIn.onPlayerTriggerEnter.RemoveListener(OnPlayerTriggerEnter);
    }

    protected override bool CheckTransitionOutCondition()
    {
        return Input.GetButtonDown(trackRightGrapple ? "Fire2" : "Fire1");
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
