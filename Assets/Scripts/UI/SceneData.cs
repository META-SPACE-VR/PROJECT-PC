using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public int sceneIndex = -1;

    private void Awake()
    {
        var obj = FindObjectsOfType<SceneData>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSceneIndex(int sceneIdx)
    {
        sceneIndex = sceneIdx;
    }
}
