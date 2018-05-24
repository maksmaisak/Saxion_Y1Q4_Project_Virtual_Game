using UnityEngine;
using System.Collections;

public class CheckpointTracker : SimpleSingleton<CheckpointTracker>
{
    public static bool WereAnyActivated { get; private set; }
    public static Vector3 LatestActivatedCheckpointPosition { get; private set; }

    public CheckpointTracker()
    {
        //clear data on scene change.
        SceneHelper.Instance.OnActiveSceneChange += OnNewSceneLoad;
    }

    public void RecordCheckpointActivation(Checkpoint checkpoint)
    {
        LatestActivatedCheckpointPosition = checkpoint.transform.position;
        WereAnyActivated = true;
    }

    private void OnNewSceneLoad()
    {
        WereAnyActivated = false;
    }
}
