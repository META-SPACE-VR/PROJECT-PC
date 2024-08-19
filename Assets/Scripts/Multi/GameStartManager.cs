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
        
        public void onClickStartBtn()
        {
            if (Instance.Runner == null) return;

            Instance.Runner.LoadScene(SceneRef.FromIndex(GAME_SCENE));
            StartBtnCanvas.SetActive(false);
        }
    }
}