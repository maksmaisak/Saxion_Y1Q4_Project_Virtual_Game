using UnityEngine;
using System.Collections;

#pragma warning disable 0649

public class ShieldHint : TutorialHint {

    [SerializeField] float delayAfterShield = 1f;

    private bool didShoot;

    void Update() {

        if (isTransitionedIn && !didShoot && IsUsingShield()) {

            didShoot = true;
            Invoke("TransitionOut", delayAfterShield);
        }
    }

    private bool IsUsingShield() {

        return Input.GetButtonDown("Shield");
    }
}
