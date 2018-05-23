using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Threat : MonoBehaviour {

    void Update() {
        
        IntensityChecker.Instance.RegisterThreat(transform.position);
    }
}
