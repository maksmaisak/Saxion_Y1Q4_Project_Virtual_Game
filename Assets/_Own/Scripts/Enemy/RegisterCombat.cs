using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterCombat : MonoBehaviour
{
    [HideInInspector] public bool detectedPlayer = false;

    // Update is called once per frame
    void Update()
    {
        if (detectedPlayer)
        {
            IntensityChecker.Instance.RegisterCombat();
        }
    }
}
