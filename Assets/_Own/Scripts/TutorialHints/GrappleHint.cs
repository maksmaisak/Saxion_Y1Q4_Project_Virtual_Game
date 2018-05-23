using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class GrappleHint : TutorialHint
{
    [SerializeField] bool leftOrRight;

    protected override bool CheckTransitionOutCondition()
    {
        return Input.GetButtonDown(leftOrRight ? "Fire1" : "Fire2");
    }
}
