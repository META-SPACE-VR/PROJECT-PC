using System.Collections;
using System.Collections.Generic;
using Fusion;
using FusionExamples.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class LevelManager : NetworkSceneManagerDefault
    {
        // 씬의 번호를 상수로 정의
        public const int LAUNCH_SCENE = 0;  // 게임 시작 씬
        public const int LOBBY_SCENE = 0;   // 대기실 씬
        
        // UI 화면과 페이더(화면 전환 효과) 변수
        [SerializeField] private UIScreen _dummyScreen;  // 더미 화면 (게임 중에 빈 화면을 보여줌)
        [SerializeField] private UIScreen _lobbyScreen;  // 대기실 화면
        [SerializeField] private CanvasFader fader;      // 화면 페이드 효과

        // LevelManager의 싱글톤 인스턴스를 반환
        public static LevelManager Instance => Singleton<LevelManager>.Instance;

        // 메뉴를 로드하는 메소드
        public static void LoadMenu()
        {
            Instance.Runner.LoadScene(SceneRef.FromIndex(LOBBY_SCENE));  // 대기실 씬을 로드함
        }

        // 특정 씬을 로드하는 메소드
        public static void LoadTrack(int sceneIndex)
        {
            Instance.Runner.LoadScene(SceneRef.FromIndex(sceneIndex));  // 지정된 씬을 로드함
        }

        // 씬 로딩 과정의 코루틴
        protected override IEnumerator LoadSceneCoroutine(SceneRef sceneRef, NetworkLoadSceneParameters sceneParams)
        {
            Debug.Log($"Loading scene {sceneRef} asdfasdasf");  // 현재 로드 중인 씬의 정보를 로그에 남김

            PreLoadScene(sceneRef.AsIndex);  // 씬 로드 전에 실행할 작업
            
            yield return base.LoadSceneCoroutine(sceneRef, sceneParams);  // 기본 씬 로드 과정 실행
            
            // 씬 로드가 끝난 후 한 프레임 대기
            yield return null;
            
            // 로드한 씬이 게임 씬이면 플레이어를 스폰
            if (sceneRef.AsIndex > LOBBY_SCENE)
            {
                if (Runner.GameMode == GameMode.Host)  // 현재 게임 모드가 호스트일 경우
                {
                    var playersArray = RoomPlayer.Players.ToArray();
                    Debug.Log(playersArray.Length);

                    foreach (var player in playersArray)
                    {
                        player.GameState = RoomPlayer.EGameState.GameCutscene;  // 플레이어의 게임 상태를 컷씬으로 변경
                        GameManager.CurrentMap.SpawnPlayer(Runner, player);  // 플레이어를 트랙에 스폰
                    }
                }
            }

            PostLoadScene();  // 씬 로드 후 실행할 작업
        }

        // 씬 로드 전에 실행할 작업
        private void PreLoadScene(int scene)
        {
            if (scene > LOBBY_SCENE)
            {
                // 대기실 씬이 아닐 때 더미 화면을 보여줌
                Debug.Log("Showing Dummy");
                UIScreen.Focus(_dummyScreen);  // 더미 화면에 포커스를 맞춤
            }
            else if(scene == LOBBY_SCENE)
            {
                // 대기실 씬일 때 모든 플레이어의 준비 상태를 해제하고 대기실 화면을 보여줌
                foreach (RoomPlayer player in RoomPlayer.Players)
                {
                    player.IsReady = false;
                }
                UIScreen.activeScreen.BackTo(_lobbyScreen);  // 대기실 화면으로 돌아감
            }
            else
            {
                UIScreen.BackToInitial();  // 초기 화면으로 돌아감
            }
            fader.gameObject.SetActive(true);  // 페이더 활성화
            fader.FadeIn();  // 화면을 서서히 나타나게 함
        }
    
        // 씬 로드 후 실행할 작업
        private void PostLoadScene()
        {
            fader.FadeOut();  // 화면을 서서히 사라지게 함
        }
    }
}