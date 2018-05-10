using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    
    public void Activate()
    {
        CheckpointManager.instance.activeChecpoints.Add(this.gameObject);
    }


}
