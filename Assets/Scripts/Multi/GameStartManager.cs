using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using FusionExamples.Utility;
using TMPro.TextMeshProUGUI;

namespace Managers
{
        public class GameStartManager : NetworkSceneManagerDefault
    {
        public const int LAUNCH_SCENE = 0;
		public const int LOBBY_SCENE = 1;
        public const int GAME_SCENE = 2; 
        public TMP_Button StartBtn; 
        
        public static GameStartManager Instance => Singleton<GameStartManager>.Instance;
        
        void Awake()
        {
            // 현재 씬의 인덱스를 얻기
            Scene currentScene = SceneManager.GetActiveScene();
            int sceneIndex = currentScene.buildIndex;
            // if (sceneIndexIndex==1) StartBtn.SetActive(true); 
        }

        public void onClickStartBtn()
        {
            Instance.Runner.LoadScene(SceneRef.FromIndex(GAME_SCENE));
        }
    }
}