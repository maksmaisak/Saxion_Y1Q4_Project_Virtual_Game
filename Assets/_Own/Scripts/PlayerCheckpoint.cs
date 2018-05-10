using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{



    private void Start()
    {
        if (CheckpointManager.Instance.ActiveChecpoints.Count > 0 && CheckpointManager.Instance.ActiveChecpoints != null && this != null)
        {
            transform.position = CheckpointManager.Instance.CheckpointPos;
        }
    }

    private void Update()
    {
        //Testing (this will be used when the player reaches the door)
        if (Input.GetKeyDown(KeyCode.R))
        {
            CheckpointManager.RefreshCheckpoints();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Checkpoint")
        {
            collision.gameObject.GetComponent<Checkpoint>().Activate();
            CheckpointManager.Instance.CheckpointPos = collision.transform.position;
        }
    }
}
