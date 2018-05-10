using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Makes sure the player respawns at the last activated checkpoint.
/// HACK TODO FIX later. The entire checkpoint respawning thing needs much more work than this.
public class PlayerCheckpoint : MonoBehaviour
{
    private void Start()
    {
        if (Checkpoint.LatestActiveCheckpointPosition.HasValue)
        {
            transform.position = Checkpoint.LatestActiveCheckpointPosition.Value;
        }
    }
}
