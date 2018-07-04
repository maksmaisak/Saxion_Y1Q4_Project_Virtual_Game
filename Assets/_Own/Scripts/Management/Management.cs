using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Management : MonoBehaviour
{
    public static Management Instance { get; private set; }

    public static void MakeSureManagementIsPresentInTheScene()
    {
        if (Instance) return;
        
        #if UNITY_EDITOR
        
        GameObject prefabGameObject = GetPrefabGameObject();
        Assert.IsTrue(PrefabUtility.GetPrefabType(prefabGameObject) == PrefabType.Prefab);
        
        var prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabGameObject);
        Assert.IsNotNull(prefabInstance);
        Assert.IsTrue(PrefabUtility.GetPrefabType(prefabInstance) == PrefabType.PrefabInstance);
        prefabInstance.SetActive(true);
        
        #endif
    }

    void Awake()
    {
        if (!Application.isPlaying && Instance != null)
        {
            if (!Instance.gameObject.scene.IsValid())
            {
                Instance = null;
            }
        }

        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroying an extra instance of Management");
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else 
            {
                DestroyImmediate(gameObject);
            }
            return;
        }

        Instance = this;

        transform.SetAsFirstSibling();

        #if UNITY_EDITOR
        PrefabUtility.ResetToPrefabState(gameObject);
        #endif

        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    
    #if UNITY_EDITOR

    /*public static void TrySaveChanges()
    {
        if (Application.isPlaying) return;
        if (Instance == null) return;

        Assert.IsTrue(PrefabUtility.GetPrefabType(Instance.gameObject) == PrefabType.PrefabInstance, "Expected PrefabType.PrefabInstance, got " + PrefabUtility.GetPrefabType(Instance.gameObject));
        GameObject prefabGameObject = PrefabUtility.ReplacePrefab(Instance.gameObject, GetPrefabGameObject(), ReplacePrefabOptions.ConnectToPrefab);
        Assert.IsTrue(PrefabUtility.GetPrefabType(prefabGameObject) == PrefabType.Prefab);

        //var ownIdManager = Instance.gameObject.GetComponentInChildren<GlobalIdManager>();
        //var prefabIdManager = prefabGameObject.GetComponentInChildren<GlobalIdManager>();
    }*/

    private static GameObject GetPrefabGameObject()
    {
        var prefabGameObject = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Own/Prefabs/_Management.prefab");
        Assert.IsNotNull(prefabGameObject);
        return prefabGameObject;
    }
    
    #endif
}
