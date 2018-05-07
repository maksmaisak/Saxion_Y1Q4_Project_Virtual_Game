using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

public class GrappleController : MonoBehaviour {

    [SerializeField] Grapple grappleLeft;
    [SerializeField] Grapple grappleRight;

    [SerializeField] float grappleShootSpeed = 100f;

    void Start() {

        Assert.IsNotNull(grappleLeft);
        Assert.IsNotNull(grappleRight);
    }

    void Update() {

        if (Input.GetButtonDown("Fire1")) {
            grappleLeft.Shoot(Camera.main.transform.forward * grappleShootSpeed);
        } 

        if (Input.GetButtonUp("Fire1")) {
            grappleLeft.Retract();
        }

        if (Input.GetButtonDown("Fire2")) {
            grappleRight.Shoot(Camera.main.transform.forward * grappleShootSpeed);
        }

        if (Input.GetButtonUp("Fire2")) {
            grappleRight.Retract();
        }
    }
}
