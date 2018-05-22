using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.FirstPerson;

#pragma warning disable 0649

/// Creates a physics joint between itself and origin upon making contact.
[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class Grapple : MonoBehaviour
{
    public enum State
    {
        Retracted,
        Flying,
        Connected,
        Retracting
    }

    [SerializeField] Transform attachmentPoint;
    [SerializeField] Rigidbody attachmentRigidbody;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip throwSound;
    [SerializeField] AudioClip defaultHookHitSound;
    [Space]
    [SerializeField] float minRopeLength = 1f;
    [SerializeField] float moveTolerance = 2f;
    [SerializeField] float springForce = 1000f;
    [SerializeField] float retractionSpeed = 100f;
    [SerializeField] float maxFlyingDistance = 40f;
    [SerializeField] float grappleAssistRadiusPerUnitDistance = 0.1f;
    [SerializeField] float canReachWithoutAssistCheckRadius = 0.1f;

    private new Rigidbody rigidbody;
    private RigidbodyFirstPersonController firstPersonController;

    public State state { get; private set; }

    private SpringJoint chainJoint;
    private FixedJoint hookJoint;
    private Grappleable grappledGrappleable;

    private LayerMask grappleAssistLayerMask;

    public bool isRetracted
    {
        get { return state == State.Retracted; }
    }

    public bool isConnected
    {
        get { return state == State.Connected; }
    }

    public float ropeLength
    {
        get
        {
            if (state != State.Connected)
            {
                throw new System.InvalidOperationException("Can't get the rope length a grapple that's not grappling anything!");
            }

            return chainJoint.maxDistance;
        }
        set 
        {
            if (state != State.Connected)
            {
                throw new System.InvalidOperationException("Can't set the rope length a grapple that's not grappling anything!");
            }

            if (value < minRopeLength) return; 
            chainJoint.maxDistance = value;
            chainJoint.minDistance = Mathf.Max(minRopeLength, value - moveTolerance);
        }
    }

    void Start()
    {
        Assert.IsNotNull(attachmentPoint);
        Assert.IsNotNull(attachmentRigidbody);

        audioSource = audioSource ?? GetComponent<AudioSource>();
        Assert.IsNotNull(audioSource);

        firstPersonController = GetComponentInParent<RigidbodyFirstPersonController>();
        Assert.IsNotNull(firstPersonController);

        rigidbody = GetComponent<Rigidbody>();

        grappleAssistLayerMask = GetGrappleAssistLayerMask();

        // Disable collisions between this and its holder.
        Collider[] ownerColliders = attachmentRigidbody.GetComponentsInChildren<Collider>();
        Collider[] ownColliders = GetComponentsInChildren<Collider>();
        foreach (Collider ownerCollider in ownerColliders)
        {
            foreach (Collider ownCollider in ownColliders)
            {
                if (ownerCollider == ownCollider) continue;
                Physics.IgnoreCollision(ownerCollider, ownCollider);
            }
        }
    }

    void FixedUpdate()
    {
        if (state == State.Flying)
        {
            if (GetDistanceFromAttachmentPoint() > maxFlyingDistance)
            {
                Retract();
                return;
            }

            AttractToGrappleables();
        }
        else if (state == State.Connected)
        {
            if (chainJoint.connectedBody == null)
            {
                Retract();
                return;
            }

            float currentDistance = Vector3.Distance(attachmentRigidbody.position, rigidbody.position);
            ropeLength = Mathf.Max(minRopeLength, Mathf.Min(ropeLength, currentDistance));
        }
    }

    void Update()
    {
        if (state == State.Retracting)
        {
            float maxDistanceChange = retractionSpeed * Time.deltaTime;
            float currentDistance = Vector3.Distance(attachmentPoint.position, transform.position);

            if (currentDistance < maxDistanceChange)
            {
                RetractImmediate();
            }
            else
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    attachmentPoint.position,
                    maxDistanceChange
                );
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Flying) return;

        // Fix in place
        //rigidbody.isKinematic = true;
        transform.position = collision.contacts[0].point;

        hookJoint = rigidbody.gameObject.AddComponent<FixedJoint>();
        hookJoint.enablePreprocessing = false;
        if (collision.rigidbody != null)
        {
            hookJoint.connectedBody = collision.rigidbody;
            hookJoint.enableCollision = false;
        }
        else
        {
            rigidbody.isKinematic = true;
        }

        // Create the joints
        var targetRigidbody = collision.rigidbody;
        if (targetRigidbody == null) targetRigidbody = rigidbody;

        chainJoint = attachmentRigidbody.gameObject.AddComponent<SpringJoint>();
        float currentDistance = Vector3.Distance(attachmentRigidbody.position, targetRigidbody.position);
        chainJoint.maxDistance = currentDistance;
        chainJoint.minDistance = Mathf.Max(minRopeLength, currentDistance - moveTolerance);
        chainJoint.tolerance = 0.5f;
        chainJoint.autoConfigureConnectedAnchor = false;
        chainJoint.anchor = Vector3.zero;
        chainJoint.connectedAnchor = Vector3.zero;
        chainJoint.spring = springForce;
        chainJoint.enablePreprocessing = false;
        chainJoint.enableCollision = true;
        chainJoint.connectedBody = targetRigidbody;

        grappledGrappleable = collision.gameObject.GetComponent<Grappleable>();
        if (grappledGrappleable != null)
        {
            grappledGrappleable.Grapple();
        }

        // Play a hit sound
        var customSound = collision.gameObject.GetComponentInParent<CustomGrappleHookHitSound>();
        var sound = customSound != null ? customSound.audioClip : defaultHookHitSound;
        sound = sound ?? defaultHookHitSound;
        if (sound != null)
        {
            audioSource.PlayOneShot(sound);
        }

        // Change state
        state = State.Connected;
    }

    void OnDestroy()
    {
        if (state == State.Connected)
        {
            Disconnect();
        }
    }

    public void Shoot(Vector3 targetPosition, float speed)
    {
        if (state != State.Retracted) RetractImmediate();

        transform.SetParent(null, worldPositionStays: true);

        rigidbody.isKinematic = false;
        rigidbody.velocity = (targetPosition - transform.position).normalized * speed;

        if (throwSound != null)
        {
            audioSource.PlayOneShot(throwSound);
        }

        state = State.Flying;
    }

    public void RetractImmediate()
    {
        if (state == State.Retracted) return;
        if (state == State.Connected) Disconnect();

        rigidbody.isKinematic = true;

        transform.SetParent(attachmentPoint, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale    = Vector3.one;

        state = State.Retracted;
    }

    public void Retract()
    {
        if (state == State.Retracted) return;
        if (state == State.Retracting) return;
        if (state == State.Connected) Disconnect();

        rigidbody.isKinematic = true;

        transform.SetParent(null);

        state = State.Retracting;
    }

    private void Disconnect()
    {
        if (chainJoint != null)
        {
            Destroy(chainJoint);
            chainJoint = null;
        }

        if (hookJoint != null)
        {
            Destroy(hookJoint);
            hookJoint = null;
        }

        if (grappledGrappleable != null)
        {
            grappledGrappleable.Ungrapple();
            grappledGrappleable = null;
        }
    }

    private LayerMask GetGrappleAssistLayerMask()
    {
        int thisLayer = gameObject.layer;
        int mask = 0;
        for (int i = 0; i < 32; ++i)
        {
            if (!Physics.GetIgnoreLayerCollision(thisLayer, i))
            {
                mask |= 1 << i;
            }
        }

        mask &= ~(1 << Physics.IgnoreRaycastLayer);
        return mask;
    }

    private void AttractToGrappleables()
    {
        float speed = rigidbody.velocity.magnitude;
        float distanceFromAttachmentPoint = GetDistanceFromAttachmentPoint();

        // TODO prioritize enemies
        // TODO check if can hit without assist.
        RaycastHit hit;
        Ray forwardRay = new Ray(rigidbody.position, rigidbody.velocity.normalized);

        bool canReachWithoutAssist = Physics.SphereCast(
            forwardRay,
            canReachWithoutAssistCheckRadius,
            maxFlyingDistance - distanceFromAttachmentPoint, 
            grappleAssistLayerMask, 
            QueryTriggerInteraction.Ignore
        );
        if (canReachWithoutAssist) 
        {
            Debug.Log("canReachWithoutAssist == true");
            return;
        }

        float assistRadius = distanceFromAttachmentPoint * grappleAssistRadiusPerUnitDistance;
        bool didHit = Physics.SphereCast(
            forwardRay,
            assistRadius, 
            out hit,
            speed * Time.fixedDeltaTime, 
            grappleAssistLayerMask, 
            QueryTriggerInteraction.Ignore
        );
        if (!didHit) return;

        rigidbody.velocity = (hit.point - rigidbody.position).normalized * speed;
    }

    private float GetDistanceFromAttachmentPoint()
    {
        return Vector3.Distance(attachmentRigidbody.position, rigidbody.position);
    }
}
