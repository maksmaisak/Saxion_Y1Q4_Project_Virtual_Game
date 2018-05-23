using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensityChecker : Singleton<IntensityChecker> {

    //[SerializeField] float minThreatDistance = 10f;

    public float distanceToClosestThreat { get; private set; }
    public float timeSinceLastCombat {
        get {
            return Time.time - timeOfLastCombat;
        }
    }

    private float currentClosestThreatDistance = float.PositiveInfinity;
    private float timeOfLastCombat = float.NegativeInfinity;

    IntensityChecker() {
        
        distanceToClosestThreat = float.PositiveInfinity;
    }

    void Update() {

        distanceToClosestThreat = currentClosestThreatDistance;
        currentClosestThreatDistance = float.PositiveInfinity;
    }

    public void RegisterThreat(Vector3 position) {

        float distance = (transform.position - position).magnitude;
        if (distance < currentClosestThreatDistance) {
            currentClosestThreatDistance = distance;
        }
    }

    public void RegisterCombat() {

        timeOfLastCombat = Time.time;
    }
}
