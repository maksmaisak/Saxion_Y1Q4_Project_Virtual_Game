using UnityEngine;
using System.Collections;

public class GrapplePullHint : TutorialHint
{
    [SerializeField] bool leftOrRight;

    protected override bool CheckTransitionOutCondition()
    {
        return Input.GetButton(leftOrRight ? "Grapple Pull Left" : "Grapple Pull Right");
    }
}
