using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class ShootHint : TutorialHint {

    [SerializeField] float delayAfterShot = 1f;

    private bool didShoot;

    void Update() {
		
        if (isTransitionedIn && !didShoot && IsShooting()) {
            
            didShoot = true;
            Invoke("TransitionOut", delayAfterShot);
        }
	}

    private bool IsShooting() {

        return Input.GetButtonDown("Fire1");
    }
}
