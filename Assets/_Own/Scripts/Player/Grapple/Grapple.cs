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
    [SerializeField] AudioClip hitSound;
    [Space]
    [SerializeField] float minRopeLength = 1f;
    [SerializeField] float springForce = 1000f;
    [SerializeField] float retractionSpeed = 100f;
    [SerializeField] float maxFlyingDistance = 40f;

    private new Rigidbody rigidbody;
    private RigidbodyFirstPersonController firstPersonController;

    public State state { get; private set; }

    private SpringJoint chainJoint;
    private FixedJoint hookJoint;
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
            if (Vector3.Distance(attachmentRigidbody.position, rigidbody.position) > maxFlyingDistance)
            {
                Retract();
            }
        }
        else if (state == State.Connected)
        {
            if (chainJoint.connectedBody == null)
            {
                Retract();
                return;
            }

            float currentDistance = Vector3.Distance(attachmentRigidbody.position, rigidbody.position);

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
        chainJoint.minDistance = 0f;
        chainJoint.maxDistance = currentDistance;

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
}
