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
        
        public static GameStartManager Instance => Singleton<GameStartManager>.Instance;

        // 씬이 로드된 후 호출되는 메서드
        protected override IEnumerator OnSceneLoaded(SceneRef sceneRef, Scene scene, NetworkLoadSceneParameters sceneParams)
        {
            yield return base.OnSceneLoaded(sceneRef, scene, sceneParams);
            yield return null;

            // 씬이 로딩된 후에 수행할 작업을 여기에 작성합니다.
            Debug.Log($"씬이 로드되었습니다: {sceneRef}");

            if (Instance.Runner!=null && Instance.Runner.IsServer && sceneRef.AsIndex == LOBBY_SCENE)
                StartBtnCanvas.SetActive(true);
        }

        public void onClickStartBtn()
        {
            Instance.Runner.LoadScene(SceneRef.FromIndex(GAME_SCENE));
            StartBtnCanvas.SetActive(false);
        }
    }
}