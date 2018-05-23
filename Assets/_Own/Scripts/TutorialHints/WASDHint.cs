using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDHint : TutorialHint
{
    protected override bool CheckTransitionOutCondition()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical"))   > 0.01f) return true;
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f) return true;
        return false;
    }
}
