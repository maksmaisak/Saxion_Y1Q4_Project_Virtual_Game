using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(KeyCode.F))
        {
            Destroy(this.gameObject);
        }
	}
}
