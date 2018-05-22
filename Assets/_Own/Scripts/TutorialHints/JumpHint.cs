using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

public class JumpHint : TutorialHint
{
    [SerializeField] TriggerEvents trigger;

    protected override void Start()
    {
        base.Start();

        Assert.IsNotNull(trigger);
        trigger.onPlayerTriggerEnter.AddListener(OnPlayerTriggerEnter);
    }

    void OnDestroy()
    {
        trigger.onPlayerTriggerEnter.RemoveListener(OnPlayerTriggerEnter);
    }

    protected override bool CheckTransitionOutCondition()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private void OnPlayerTriggerEnter()
    {
        if (!isTransitionedIn)
        {
            trigger.onPlayerTriggerEnter.RemoveListener(OnPlayerTriggerEnter);
            TransitionIn();
        }
    }
}
