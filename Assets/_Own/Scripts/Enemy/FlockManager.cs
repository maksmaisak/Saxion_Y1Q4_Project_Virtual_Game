using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FlockManager : MonoBehaviour {


   // public static GameObject[] enemyArray;
    public static List<GameObject> enemyList;
	// Use this for initialization
	void Start () {
        enemyList = GameObject.FindGameObjectsWithTag("Enemy").ToList();
	}
   	
}
