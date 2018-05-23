using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class MouseHint : TutorialHint
{
    protected override bool CheckTransitionInCondition()
    {
        return true;
    }

    protected override bool CheckTransitionOutCondition()
    {
        if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.01f) return true;
        if (Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.01f) return true;
        return false;
    }
}
