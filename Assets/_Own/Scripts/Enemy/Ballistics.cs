using UnityEngine;
using System.Collections;
using System;

public static class Ballistics {

    public static Vector3? GetStartVelocity(
        Vector3 start,
        Vector3 target,
        float muzzleSpeed
    ) {

        Vector3 delta = target - start;
        Vector3 flatDelta = new Vector3(delta.x, 0f, delta.z);
        Vector3 direction = flatDelta.normalized;

        float x = flatDelta.magnitude;
        float y = delta.y;
        float v = muzzleSpeed;
        float vSqr = v * v;
        float g = Mathf.Abs(Physics.gravity.y);

        float angle = Mathf.Atan((vSqr - Mathf.Sqrt(vSqr * vSqr - g * (g * x * x + 2f * y * vSqr))) / (x * g));
        if (float.IsNaN(angle)) return null;

        Vector3 startVelocity = direction * muzzleSpeed * Mathf.Cos(angle);
        startVelocity.y = muzzleSpeed * Mathf.Sin(angle);

        return startVelocity;
    }
}
