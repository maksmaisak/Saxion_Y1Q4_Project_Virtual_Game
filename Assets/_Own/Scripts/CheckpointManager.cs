using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{

    public static CheckpointManager Instance = null;

    public List<GameObject> ActiveChecpoints;
    public Vector3 CheckpointPos;

    void Awake()
    {
        if (Instance == null)
        {

            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }


        DontDestroyOnLoad(this.gameObject);

        ActiveChecpoints = new List<GameObject>();
    }

    public static void RefreshCheckpoints()
    {
        if (Instance.ActiveChecpoints != null)
        {
            Instance.ActiveChecpoints.Clear();
        }
    }
}
