using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.FirstPerson;

#pragma warning disable 0649

/// Handles grapple-related player input. 
public class GrappleController : MonoBehaviour
{
    [SerializeField] Grapple grappleLeft;
    [SerializeField] Grapple grappleRight;

    [SerializeField] Image crosshairIndicator;

    [SerializeField] float grappleShootSpeed = 100f;
    [SerializeField] float grappleMinRange = 0.6f;
    [SerializeField] float grappleMaxRange = 40f;

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

        // FIXME Hardcoded keycodes
        bool shouldPullLeft  = Input.GetKey(KeyCode.Q);
        bool shouldPullRight = Input.GetKey(KeyCode.E);
        bool shouldPullAtLeastOne = shouldPullLeft || shouldPullRight;

        bool onlyOneGrappleConnected = grappleLeft.isConnected != grappleRight.isConnected;
        if (onlyOneGrappleConnected && shouldPullAtLeastOne) 
        {
            shouldPullLeft = shouldPullRight = true;
        }

        if (shouldPullLeft && grappleLeft.isConnected)
        {
            grappleLeft.ropeLength -= grapplePullingSpeed * Time.deltaTime;
        }

        if (shouldPullRight && grappleRight.isConnected)
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
            if (hit.distance < grappleMinRange)
            {
                SetCrosshairMode(false);
                targetPosition = Vector3.zero;
                return false;
            }

            SetCrosshairMode(true);
            targetPosition = hit.point;
            return true;
        }

        SetCrosshairMode(false);
        targetPosition = ray.GetPoint(grappleMaxRange);
        return true;

        /*crosshairIndicator.color = Color.gray;
        targetPosition = Vector3.zero;
        return false;*/
    }

    private void SetCrosshairMode(bool isActive)
    {
        if (crosshairIndicator == null) return;
        crosshairIndicator.color = isActive ? Color.black : Color.gray;
    }
}
