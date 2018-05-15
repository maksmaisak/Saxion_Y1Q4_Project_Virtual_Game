using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.FirstPerson;

#pragma warning disable 0649

/// Creates a physics joint between itself and origin upon making contact.
[RequireComponent(typeof(Rigidbody), typeof(LineRenderer), typeof(AudioSource))]
public class Grapple : MonoBehaviour
{
    enum State
    {
        Retracted,
        Flying,
        Connected
    }

    [SerializeField] Transform attachmentPoint;
    [SerializeField] Rigidbody attachmentRigidbody;
    [SerializeField] AudioClip throwSound;
    [SerializeField] AudioClip hitSound;
    [Space]
    [SerializeField] float minRopeLength = 1f;
    [SerializeField] float springForce = 1000f;
    [SerializeField] float retractionSpeed = 1f;
    [SerializeField] float maxFlyingDistance = 300f;

    private new Rigidbody rigidbody;
    private RigidbodyFirstPersonController firstPersonController;
    private AudioSource audioSource;
    private LineRenderer lineRenderer;

    private State state = State.Retracted;
    private SpringJoint joint;
    private Grappleable grappledGrappleable;

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

            return joint.minDistance;
        }
        set 
        {
            if (state != State.Connected)
            {
                throw new System.InvalidOperationException("Can't set the rope length a grapple that's not grappling anything!");
            }

            if (value < minRopeLength) return; 
            joint.minDistance = joint.maxDistance = value;
        }
    }

    void Start()
    {
        Assert.IsNotNull(attachmentPoint);
        Assert.IsNotNull(attachmentRigidbody);

        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();

        firstPersonController = GetComponentInParent<RigidbodyFirstPersonController>();
        Assert.IsNotNull(firstPersonController);

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

    void Update()
    {
        if (isRetracted)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
            Vector3 delta = rigidbody.position - attachmentPoint.position;
            lineRenderer.SetPosition(0, attachmentPoint.position - delta.normalized * 1f);
            lineRenderer.SetPosition(1, rigidbody.position);
        }
    }

    void FixedUpdate()
    {
        if (state == State.Flying)
        {
            if (Vector3.Distance(attachmentRigidbody.position, rigidbody.position) > maxFlyingDistance)
            {
                Retract();
            }
        }
        else if (state == State.Connected)
        {
            float currentDistance = Vector3.Distance(attachmentRigidbody.position, joint.connectedBody.position);

            if (ropeLength > minRopeLength)
            {
                if (currentDistance < ropeLength)
                {
                    ropeLength = currentDistance;
                }
            }
            else
            {
                ropeLength = minRopeLength;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Flying) return;

        // Fix in place
        rigidbody.isKinematic = true;
        transform.position = collision.contacts[0].point;
        transform.SetParent(collision.transform, worldPositionStays: true);

        // Create the joint
        var targetRigidbody = collision.rigidbody;
        if (targetRigidbody == null) targetRigidbody = rigidbody;

        joint = attachmentRigidbody.gameObject.AddComponent<SpringJoint>();
        float currentDistance = Vector3.Distance(attachmentRigidbody.position, targetRigidbody.position);
        joint.minDistance = joint.maxDistance = currentDistance;
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = Vector3.zero;
        joint.spring = springForce;
        joint.enablePreprocessing = false;
        joint.enableCollision = true;
        joint.connectedBody = targetRigidbody;

        grappledGrappleable = collision.gameObject.GetComponent<Grappleable>();
        if (grappledGrappleable != null)
        {
            grappledGrappleable.Grapple();
        }

        // Play the hit sound
        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
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
        if (!isRetracted) Retract();

        transform.SetParent(null, worldPositionStays: true);

        rigidbody.isKinematic = false;
        rigidbody.velocity = (targetPosition - transform.position).normalized * speed;

        if (throwSound != null)
        {
            audioSource.PlayOneShot(throwSound);
        }

        state = State.Flying;
    }

    public void Retract()
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

    private void Disconnect()
    {
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }

        if (grappledGrappleable != null)
        {
            grappledGrappleable.Ungrapple();
            grappledGrappleable = null;
        }
    }
}
