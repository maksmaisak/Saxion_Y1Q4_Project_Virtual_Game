using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteeringManager))]
public class TestCompensateExternalForces : MonoBehaviour
{
    private SteeringManager steeringManager;

    void Start()
    {
        steeringManager = GetComponent<SteeringManager>();
    }

    void FixedUpdate()
    {
        steeringManager.CompensateExternalForces();
    }
}
