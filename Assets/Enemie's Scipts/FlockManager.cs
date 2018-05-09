using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour {


    public static GameObject[] enemyArray; 
	// Use this for initialization
	void Start () {
        enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
	}
	
	
}
