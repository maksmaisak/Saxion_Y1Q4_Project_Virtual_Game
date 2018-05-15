using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.FirstPerson;

#pragma warning disable 0649

public class GrappleController : MonoBehaviour
{
    [SerializeField] Grapple grappleLeft;
    [SerializeField] Grapple grappleRight;

    [SerializeField] Image crosshairIndicator;

    [SerializeField] float grappleShootSpeed = 100f;
    [SerializeField] float grappleMaxRange = 20f;

    [Tooltip("Meters per second")]
    [SerializeField] float grapplePullingSpeed = 2f;

    [SerializeField] RigidbodyFirstPersonController firstPersonController;

    void Start()
    {
        Assert.IsNotNull(grappleLeft);
        Assert.IsNotNull(grappleRight);
        Assert.IsNotNull(firstPersonController);
    }

    void Update()
    {
        RetractIfNeeded();
        ShootIfNeeded();
        PullIfNeeded();
    }

    private void RetractIfNeeded()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            grappleLeft.Retract();
        }

        if (Input.GetButtonUp("Fire2"))
        {
            grappleRight.Retract();
        }
    }

    private void ShootIfNeeded()
    {
        Vector3 targetPosition;
        if (!CheckCanShootGrapple(out targetPosition)) return;

        if (Input.GetButtonDown("Fire1"))
        {
            grappleLeft.Shoot(targetPosition, grappleShootSpeed);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            grappleRight.Shoot(targetPosition, grappleShootSpeed);
        }
    }

    private void PullIfNeeded()
    {
        if (firstPersonController.isGrounded) return;

        if (grappleLeft.isConnected && Input.GetKey(KeyCode.Q))
        {
            grappleLeft.ropeLength -= grapplePullingSpeed * Time.deltaTime;
        }

        if (grappleRight.isConnected && Input.GetKey(KeyCode.E))
        {
            grappleRight.ropeLength -= grapplePullingSpeed * Time.deltaTime;
        }
    }

    private bool CheckCanShootGrapple(out Vector3 targetPosition)
    {
        Transform cameraTransform = Camera.main.transform;
        var ray = new Ray(cameraTransform.position, cameraTransform.forward);

        RaycastHit hit;
        bool didHit = Physics.Raycast(
            ray,
            out hit,
            grappleMaxRange
        );

        if (didHit)
        {
            crosshairIndicator.color = Color.black;
            targetPosition = hit.point;
            return true;
        }

        crosshairIndicator.color = Color.gray;
        targetPosition = Vector3.zero;
        return false;
    }
}
