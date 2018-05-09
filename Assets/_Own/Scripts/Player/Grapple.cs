using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.FirstPerson;

#pragma warning disable 0649

/// Creates a physics joint between itself and origin upon making contact.
[RequireComponent(typeof(Rigidbody), typeof(LineRenderer))]
public class Grapple : MonoBehaviour {

    enum State {
        Retracted,
        Flying,
        Connected
    }

    [SerializeField] Transform attachmentPoint;
    [SerializeField] Rigidbody attachmentRigidbody;
    [Space]
    [SerializeField] float minDistance = 1f;
    [SerializeField] float springForce = 1000f;
    [SerializeField] float retractionSpeed = 1f;
    [SerializeField] float maxFlyingDistance = 300f;

    private new Rigidbody rigidbody;
    private RigidbodyFirstPersonController firstPersonController;
    private LineRenderer lineRenderer;

    private State state = State.Retracted;
    private SpringJoint joint;

    public bool isRetracted {
        get { return state == State.Retracted; }
    }

    void Start() {

        Assert.IsNotNull(attachmentPoint);
        Assert.IsNotNull(attachmentRigidbody);

        rigidbody = GetComponent<Rigidbody>();
        firstPersonController = GetComponentInParent<RigidbodyFirstPersonController>();
        lineRenderer = GetComponent<LineRenderer>();

        // Disable collisions between this and its holder.
        Collider[] ownerColliders = attachmentRigidbody.GetComponentsInChildren<Collider>();
        Collider[] ownColliders = GetComponentsInChildren<Collider>();
        foreach (Collider ownerCollider in ownerColliders) {
            foreach (Collider ownCollider in ownColliders) {
                if (ownerCollider == ownCollider) continue;
                Physics.IgnoreCollision(ownerCollider, ownCollider);
            }
        }
    }

    void Update() {

        if (isRetracted) {
            
            lineRenderer.enabled = false;
        } else {

            lineRenderer.enabled = true;
            Vector3 delta = rigidbody.position - attachmentPoint.position;
            lineRenderer.SetPosition(0, attachmentPoint.position - delta.normalized * 1f);
            lineRenderer.SetPosition(1, rigidbody.position);
        }
    }

    void FixedUpdate() {

        if (state == State.Flying) {

            if (Vector3.Distance(attachmentRigidbody.position, rigidbody.position) > maxFlyingDistance) {
                
                Retract();
            }
        }

        if (joint != null) {
            
            float ropeLength = joint.minDistance;
            float currentDistance = Vector3.Distance(attachmentRigidbody.position, joint.connectedBody.position);

            if (ropeLength > minDistance) {
                
                if (currentDistance < ropeLength) {
                    ropeLength = currentDistance;
                } else if (!firstPersonController.Grounded) {
                    ropeLength -= retractionSpeed * Time.fixedDeltaTime;
                    if (ropeLength < minDistance) ropeLength = minDistance;
                }

            } else {
                
                ropeLength = minDistance;
            }

            joint.minDistance = joint.maxDistance = ropeLength;
        }
    }

    void OnCollisionEnter(Collision collision) {

        if (state != State.Flying) return;

        // Fix in place
        rigidbody.isKinematic = true;
        transform.position = collision.contacts[0].point;
        transform.SetParent(collision.transform, worldPositionStays: true);

        // Create the joint
        var targetRigidbody = collision.gameObject.GetComponent<Rigidbody>();
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

        // Change state
        state = State.Connected;
    }

    public void Shoot(Vector3 targetPosition, float speed) {

        if (!isRetracted) Retract();

        transform.SetParent(null, worldPositionStays: true);

        rigidbody.isKinematic = false;
        rigidbody.velocity = (targetPosition - transform.position).normalized * speed;

        state = State.Flying;
    }

    public void Retract() {

        if (state == State.Retracted) return;

        if (joint != null) {

            Destroy(joint);
            joint = null;
        }

        rigidbody.isKinematic = true;

        transform.SetParent(attachmentPoint, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale    = Vector3.one;

        state = State.Retracted;
    }
}
