using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

/// Manages drawing a grapple rope between its gameobject and the holder.
public class GrappleRope : MonoBehaviour {

    [SerializeField] Transform origin;

	// Use this for initialization
	void Start () {

        Assert.IsNotNull(origin);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
