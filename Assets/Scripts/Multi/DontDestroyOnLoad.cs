using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public GameObject gameObjectToKeep;

    private void Awake()
    {
        // 현재 씬에 이미 해당 오브젝트가 존재하는지 확인
        GameObject[] objs = GameObject.FindGameObjectsWithTag(gameObjectToKeep.tag);
        if (objs.Length > 1)
        {
            // 씬에 중복된 오브젝트가 있을 경우, 새로 생성된 오브젝트를 파괴
            Destroy(gameObject);
        }
        else
        {
            // `DontDestroyOnLoad` 설정
            DontDestroyOnLoad(gameObject);
        }
    }
}
