
using UnityEngine;

#pragma warning disable 0649

[RequireComponent(typeof(Rigidbody))]
public class FloatingBehaviour : MonoBehaviour
{
    //[SerializeField] private float hoverMultiplier = 20f;
    //[SerializeField] private float slowdownDistance = 1f;
    [SerializeField] private float maxHoverForce = 20f;
    [SerializeField] private float upOffset = 2.5f;

    private GameObject target;
    private Rigidbody rb;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float desiredHeight = target.transform.position.y + upOffset;

        Vector3 force = Vector3.zero;

        /*float delta = desiredHeight - transform.position.y;
        force += Vector3.up * delta * hoverMultiplier;
        
        // Cancel gravity
        force += -Physics.gravity;
        */

        float desiredVerticalSpeed = target.transform.position.y + upOffset - desiredHeight;
        float desiredVerticalAcceleration = desiredVerticalSpeed - rb.velocity.y;

        force += -Physics.gravity + Vector3.up * rb.mass * desiredVerticalAcceleration;
        force = Vector3.ClampMagnitude(force, maxHoverForce);

        Debug.DrawRay(rb.position, force, Color.red);
        rb.AddForce(force);
    }
}

