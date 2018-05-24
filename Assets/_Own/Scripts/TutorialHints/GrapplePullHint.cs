using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

public class GrapplePullHint : TutorialHint
{
    [SerializeField] bool trackRightGrapple;

    private Grapple grapple;
    private string pullButtonName;

    protected override void Start()
    {
        base.Start();

        var grappleController = Player.Instance.GetComponentInChildren<GrappleController>();
        Assert.IsNotNull(grappleController);

        if (trackRightGrapple)
        {
            grapple = grappleController.rightGrapple;
            pullButtonName = "Grapple Pull Right";
        }
        else 
        {
            grapple = grappleController.leftGrapple;
            pullButtonName = "Grapple Pull Left";
        }

        Assert.IsNotNull(grapple);
    }

    protected override bool CheckTransitionOutCondition()
    {
        return grapple.isConnected && Input.GetButton(pullButtonName);
    }
}
