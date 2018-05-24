using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorActivationScript : MonoBehaviour
{
    [SerializeField] private float maxDegreesPerSecond= 2;
    [HideInInspector] public bool isActivated = false;

    void Start()
    {
        //Activate();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            isActivated = true;
        }

        if(isActivated)
        {
            Activate();
        }
    }

    public void Activate()
    {

        foreach (Transform child in transform)
        {
            child.transform.rotation = Quaternion.RotateTowards(child.transform.rotation,Quaternion.identity,maxDegreesPerSecond * Time.deltaTime);
        }
    }
}
