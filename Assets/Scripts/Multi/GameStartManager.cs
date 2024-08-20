using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Fusion;
using FusionExamples.Utility;

namespace Managers
{
        public class GameStartManager : NetworkSceneManagerDefault
    {
        public const int LAUNCH_SCENE = 0;
		public const int LOBBY_SCENE = 1;
        public const int GAME_SCENE = 2; 
        public GameObject StartBtnCanvas;
        public Button StartBtn; 

        //생화학자, 기계공, 전기공, 의사 
        string[] jobList = {"Biochemist","Mechanic","Electrician","Doctor"};
        List<string> availableJobList; //남은 직업을 담을 리스트 
        
        public static GameStartManager Instance => Singleton<GameStartManager>.Instance;

        // 씬이 로드된 후 호출되는 메서드
        protected override IEnumerator OnSceneLoaded(SceneRef sceneRef, Scene scene, NetworkLoadSceneParameters sceneParams)
        {
            yield return base.OnSceneLoaded(sceneRef, scene, sceneParams);
            yield return null;

            // 씬이 로딩된 후에 수행할 작업을 여기에 작성합니다.
            Debug.Log($"씬이 로드되었습니다: {sceneRef}");

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = true;

            if (Instance.Runner!=null && Instance.Runner.IsServer && sceneRef.AsIndex == LOBBY_SCENE)
                StartBtnCanvas.SetActive(true);
        }

        public void onClickStartBtn()
        {
            Instance.Runner.LoadScene(SceneRef.FromIndex(GAME_SCENE));
            StartBtnCanvas.SetActive(false);

            availableJobList = new List<string>(jobList);
            AssignRandomJobs();
        }

        void AssignRandomJobs()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach(GameObject player in players)
            {
                Player playerComponent = player.GetComponent<Player>();
                string randomJob = GetRandomJob();
                playerComponent.Rpc_SetJob(randomJob);
            }
        }

        string GetRandomJob()
        {
            if (availableJobList.Count==0) return ""; 

            int randomIndex = Random.Range(0, availableJobList.Count);
            string randomJob = availableJobList[randomIndex];
            Debug.Log("할당된 직업: " + randomJob);

            //할당된 직업을 리스트에서 제거 (중복 할당 방지)
            availableJobList.RemoveAt(randomIndex);
            
            return randomJob; 
        }
    }
}