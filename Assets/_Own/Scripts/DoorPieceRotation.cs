using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPieceRotation : MonoBehaviour
{

    [SerializeField] private float maxRotationSpeed = 10;
    [SerializeField] private float minRotationSpeed = -10;

    private float rotationSpeed = 0;
    private DoorActivationScript door;

    void Start()
    {
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        door = transform.parent.GetComponent<DoorActivationScript>();
    }


    void Update()
    {
        if (door.isActivated == false)
        {
            RotateOverTime();
        }
    }

    void RotateOverTime()
    {
        transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * rotationSpeed);
    }
}
