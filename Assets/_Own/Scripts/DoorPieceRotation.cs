using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPieceRotation : MonoBehaviour
{

    [SerializeField] private float maxRotationSpeed = 10;
    [SerializeField] private float minRotationSpeed = -10;
    [SerializeField] private GameObject door;

    private DoorActivationScript doorActivationScript;

    private float rotationSpeed = 0;

    void Start()
    {
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        doorActivationScript = door.GetComponent<DoorActivationScript>();
    }


    void Update()
    {
        if (doorActivationScript.isActivated == false)
        {
            RotateOverTime();
        }
    }

    void RotateOverTime()
    {
        transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * rotationSpeed);
    }
}
