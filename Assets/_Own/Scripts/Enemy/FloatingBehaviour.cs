
using UnityEngine;

#pragma warning disable 0649

[RequireComponent(typeof(Rigidbody))]
public class FloatingBehaviour : MonoBehaviour
{

    public float hoverForce = 20;

    [SerializeField] private float upOffset = 2.5f;

    private float hoverHeight;
    private GameObject target;
    private Rigidbody rb;


    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        hoverHeight = target.transform.position.y + upOffset;

        float proportionalHeight = (hoverHeight - transform.position.y) / hoverHeight;
        Vector3 _hoverForce = Vector3.up * proportionalHeight * hoverForce * Time.deltaTime;
        rb.AddForce(_hoverForce, ForceMode.Acceleration);
    }
}


