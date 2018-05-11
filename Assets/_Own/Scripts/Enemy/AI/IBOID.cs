using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBOID  {

    Vector3 GetVelocity();
    float GetMaxVelocity();
    Vector3 GetPosition();
    float GetMaxForce();
    GameObject GetEnemy();
    float GetSeparationDistance();
    float GetSeparationFactor();
    float GetCollisionAvoidanceMultiplier();
    float GetCollisionAvoidanceRange();
}
