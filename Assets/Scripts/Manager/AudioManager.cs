using System.Collections.Generic;
using FusionExamples.Utility;
using UnityEngine;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioManager : MonoBehaviour
{
    // 음악을 재생할 때 사용할 AudioSource
	public AudioSource musicSource;

    // 오디오 믹서와 그룹
	public AudioMixer masterMixer;
	public AudioMixerGroup sfxMixer; //효과음 믹서
	public AudioMixerGroup uiMixer;
	public AudioMixerGroup musicMixer;
	public DefaultMixerTarget defaultMixer = DefaultMixerTarget.None; //기본 믹서 설정

    // 볼륨을 조절하기 위한 매개변수 이름
	public static readonly string mainVolumeParam = "Volume";
	public static readonly string sfxVolumeParam = "SFXVol";
	public static readonly string uiVolumeParam = "UIVol";
	public static readonly string musicVolumeParam = "MusicVol";

	[SerializeField] private AudioBank soundBank; //효과음 저장소
	[SerializeField] private AudioBank musicBank; //음악 저장소

    // Singleton 패턴을 사용하는 AudioManager 인스턴스
	public static AudioManager Instance => Singleton<AudioManager>.Instance;

	private void Awake()
	{
		InitBanks(); // 효과음과 음악 저장소 초기화
		musicSource.outputAudioMixerGroup = musicMixer; // 음악 소스에 음악 믹서 설정
		DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 이 오브젝트는 삭제되지 않도록 설정
	}

	private void Start()
	{
		InitMixer(); // 믹서의 볼륨 초기화
	}

    // 저장소 초기화 
    private void InitBanks()
	{
		soundBank.Build(); // 효과음 저장소 빌드
		musicBank.Build(); // 음악 저장소 빌드
	}

    // 믹서의 볼륨을 설정
    private void InitMixer()
	{
		SetMixerFromPref(mainVolumeParam);
		SetMixerFromPref(musicVolumeParam);
		SetMixerFromPref(sfxVolumeParam);
		SetMixerFromPref(uiVolumeParam);
	}

	// Public Functions

    // 사운드 재생
	public static void Play(string clip, AudioMixerGroup mixerTarget, Vector3? position = null)
	{
		if (Instance.soundBank.TryGetAudio(clip, out AudioClip audioClip))
		{
			GameObject clipObj = new GameObject(clip, typeof(AudioDestroyer)); // 새로운 게임 오브젝트 생성
			AudioSource src = clipObj.AddComponent<AudioSource>(); // 오디오 소스 추가
			if (position.HasValue)
			{
				clipObj.transform.position = position.Value; //위치 설정
				src.spatialBlend = 1; //3D 소리 설정
				src.rolloffMode = AudioRolloffMode.Linear;
				src.maxDistance = 50; //최대 거리
				src.dopplerLevel = 0; //도플러 효과 비활성화 
			}
			src.clip = audioClip; //클립 설정
			src.outputAudioMixerGroup = mixerTarget; //믹서 그룹 설정
			src.Play(); //재생 시작
		}
		else
		{
			Debug.LogWarning($"AudioClip '{clip}' not present in audio bank"); // 클립이 저장소에 없을 경우 경고
		}
	}

	public static void Play(string clip, MixerTarget mixerTarget, Vector3? position = null)
	{
		Play(clip, Instance.GetMixerGroup(mixerTarget), position);
	}

	public static void Play(string clip, string mixerTarget, Vector3? position = null)
	{
		Play(clip, Instance.GetMixerGroup(mixerTarget), position);
	}

	public static void Play(string clip, Vector3? position = null)
	{
		Play(clip, MixerTarget.Default, position);
	}

	public static void PlayAndFollow(string clip, Transform target, MixerTarget mixerTarget)
	{
		if (Instance.soundBank.TryGetAudio(clip, out AudioClip audioClip))
		{
            GameObject clipObj = new GameObject(clip, typeof(AudioDestroyer)); // 새로운 게임 오브젝트 생성
            AudioSource src = clipObj.AddComponent<AudioSource>(); // 오디오 소스 추가
            FollowTarget follow = clipObj.AddComponent<FollowTarget>(); // 타겟을 따르는 컴포넌트 추가
            src.spatialBlend = 1; // 3D 소리 설정
            src.rolloffMode = AudioRolloffMode.Linear;
            src.maxDistance = 50; // 최대 거리
            src.dopplerLevel = 0; // 도플러 효과 비활성화
            src.clip = audioClip; // 클립 설정
            src.outputAudioMixerGroup = Instance.GetMixerGroup(mixerTarget); // 믹서 그룹 설정
            follow.target = target; // 타겟 설정
            src.Play(); // 재생 시작
		}
		else
		{
			Debug.LogWarning($"AudioClip '{clip}' not present in audio bank"); // 클립이 저장소에 없을 경우 경고
		}
	}

	public static void PlayAndFollow(string clip, Transform target)
	{
		PlayAndFollow(clip, target, MixerTarget.Default);
	}

	// 음악 재생 

	public static void PlayMusic(string music)
	{
		if (string.IsNullOrEmpty(music) == false)
		{
			if (Instance.musicBank.TryGetAudio(music, out AudioClip audio))
			{
				Instance.musicSource.clip = audio; // 음악 소스에 클립 설정
                Instance.musicSource.Play(); // 음악 재생 시작
			}
			else
			{
				Debug.LogWarning($"AudioClip '{music}' not present in music bank"); // 음악이 저장소에 없을 경우 경고
			}
		}
	}

	public static void StopMusic()
	{
		Instance.musicSource.Stop(); //음악 정지
		Instance.musicSource.clip = null; //클립 제거
	}

	// Volume

    public static void SetVolumeMaster(float value)
    {
        Instance.masterMixer.SetFloat(mainVolumeParam, ToDecibels(value)); // 마스터 볼륨 설정
        SetPref(mainVolumeParam, value); // 저장
    }

    public static void SetVolumeSFX(float value)
    {
        Instance.masterMixer.SetFloat(sfxVolumeParam, ToDecibels(value)); // 효과음 볼륨 설정
        SetPref(sfxVolumeParam, value); // 저장
    }

    public static void SetVolumeUI(float value)
    {
        Instance.masterMixer.SetFloat(uiVolumeParam, ToDecibels(value)); // UI 소리 볼륨 설정
        SetPref(uiVolumeParam, value); // 저장
    }

    public static void SetVolumeMusic(float value)
    {
        Instance.masterMixer.SetFloat(musicVolumeParam, ToDecibels(value)); // 음악 볼륨 설정
        SetPref(musicVolumeParam, value); // 저장
    }

    public static float ToDecibels(float value)
    {
        if (value == 0) return -80; // 볼륨이 0일 때 매우 낮은 값 설정
        return Mathf.Log10(value) * 20; // 볼륨을 데시벨로 변환
    }

	// Prefs

	// returns a linear [0-1] volume value
    private static float GetPref(string pref)
    {
        float v = PlayerPrefs.GetFloat(pref, 0.75f); // 저장된 값 가져오기, 없으면 기본값 0.75
        return v;
    }

	// sets a linear [0-1] volume value
    private static void SetPref(string pref, float val)
	{
		PlayerPrefs.SetFloat(pref, val); //저장
	}

    private void SetMixerFromPref(string pref)
	{
		masterMixer.SetFloat(pref, ToDecibels(GetPref(pref))); // 저장소에서 가져온 값으로 믹서 설정
	}

	// Mixer & Other

    private AudioMixerGroup DefaultMixerGroup()
	{
		return GetMixerGroup((MixerTarget)Instance.defaultMixer);
	}

    private AudioMixerGroup GetMixerGroup(MixerTarget target)
	{
        if (target == MixerTarget.None) return null; // 믹서 그룹이 없는 경우
        if (target == MixerTarget.Default) return GetMixerGroup((MixerTarget)defaultMixer);
        if (target == MixerTarget.SFX) return sfxMixer;
        if (target == MixerTarget.UI) return uiMixer;
        throw new System.Exception("Invalid MixerTarget"); // 잘못된 믹서 타겟
	}

    private AudioMixerGroup GetMixerGroup(string target)
	{
		AudioMixerGroup[] foundGroups = masterMixer.FindMatchingGroups(target);
		if (foundGroups.Length > 0) return foundGroups[0]; // 그룹이 있을 경우 반환
		throw new System.Exception($"No mixer group by the name {target} could be found");
	}

    // 믹서 타겟과 기본 믹서 타겟
	public enum MixerTarget { None, Default, SFX, UI }
	public enum DefaultMixerTarget { None = MixerTarget.None, SFX = MixerTarget.SFX, UI = MixerTarget.UI }

	// 오디오 저장소에서 사용하는 키-값 쌍 클래스
    [System.Serializable]
    public class BankKVP
    {
        public string Key;  // 사운드의 이름
        public AudioClip Value;  // 실제 오디오 클립
    }

    // 오디오 클립들을 저장하고 관리하는 클래스
    [System.Serializable]
    public class AudioBank
    {
        [SerializeField] private BankKVP[] kvps; // 에디터에서 설정할 수 있는 키-값 쌍 배열
        private readonly Dictionary<string, AudioClip> dictionary = new Dictionary<string, AudioClip>(); // 오디오 클립을 저장하는 사전

        // 오디오 저장소의 유효성을 검사
        public bool Validate()
        {
            if (kvps.Length == 0) return false; // 저장소에 항목이 없으면 유효하지 않음

            List<string> keys = new List<string>();
            foreach (var kvp in kvps)
            {
                if (keys.Contains(kvp.Key)) return false; // 키가 중복되면 유효하지 않음
                keys.Add(kvp.Key); // 중복되지 않은 키를 리스트에 추가
            }
            return true; // 유효한 저장소
        }

        // 오디오 저장소를 빌드
        public void Build()
        {
            if (Validate())
            {
                for (int i = 0; i < kvps.Length; i++)
                {
                    dictionary.Add(kvps[i].Key, kvps[i].Value); // 키와 오디오 클립을 사전에 추가
                }
            }
        }

        // 키로 오디오 클립을 가져오기
        public bool TryGetAudio(string key, out AudioClip audio)
        {
            return dictionary.TryGetValue(key, out audio); // 사전에서 키로 오디오 클립을 찾기
        }
    }

#if UNITY_EDITOR
    // Unity 에디터에서 AudioBank 클래스의 속성을 커스터마이즈하여 보여주기
    [CustomPropertyDrawer(typeof(AudioBank))]
    public class AudioBankDrawer : PropertyDrawer
    {
        // 속성의 높이를 결정
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("kvps"));
        }

        // 속성을 GUI로 그리기
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property.FindPropertyRelative("kvps"), label, true); // "kvps" 배열 그리기
            EditorGUI.EndProperty();
        }
    }

    // Unity 에디터에서 BankKVP 클래스의 속성을 커스터마이즈하여 보여주기
    [CustomPropertyDrawer(typeof(BankKVP))]
    public class BankKVPDrawer : PropertyDrawer
    {
        // 속성을 GUI로 그리기
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 키와 값을 각각의 공간에 그리기
            Rect rect1 = new Rect(position.x, position.y, position.width / 2 - 4, position.height);
            Rect rect2 = new Rect(position.center.x + 2, position.y, position.width / 2 - 4, position.height);

            EditorGUI.PropertyField(rect1, property.FindPropertyRelative("Key"), GUIContent.none); // 키 그리기
            EditorGUI.PropertyField(rect2, property.FindPropertyRelative("Value"), GUIContent.none); // 값 그리기

            EditorGUI.EndProperty();
        }
    }
#endif
}