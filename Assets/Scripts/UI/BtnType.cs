using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static HPhysic.Connector;

public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BTNType currentType;
    public Transform buttonScale;
    public float scale = 1.1f;

    [Header("Type Properties")]
    public CanvasGroup mainGroup;
    public CanvasGroup startGroup;

    public int stageNumber;
    public CanvasGroup stage;
    public CanvasGroup[] stages;

    public int levelNumber;
    public GameObject playButton;
    public Sprite previewImage;
    public Image preview;

    // public properties
    Vector3 defaultScale;

    // 커스텀 인스펙터
    [CustomEditor(typeof(BtnType))]
    public class CustonEditor: Editor
    {
        SerializedProperty m_buttonScale;
        SerializedProperty m_mainGroup;
        SerializedProperty m_startGroup;
        SerializedProperty m_stage;
        SerializedProperty m_stages;
        SerializedProperty m_playButton;
        SerializedProperty m_previewImage;
        SerializedProperty m_preview;

        private void OnEnable()
        {
            m_buttonScale = serializedObject.FindProperty("buttonScale");
            m_mainGroup = serializedObject.FindProperty("mainGroup");
            m_startGroup = serializedObject.FindProperty("startGroup");
            m_stage = serializedObject.FindProperty("stage");
            m_stages = serializedObject.FindProperty("stages");
            m_playButton = serializedObject.FindProperty("playButton");
            m_previewImage = serializedObject.FindProperty("previewImage");
            m_preview = serializedObject.FindProperty("preview");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var script = (BtnType)target;

            script.currentType = (BTNType)EditorGUILayout.EnumPopup("Current Type", script.currentType);
            EditorGUILayout.PropertyField(m_buttonScale, new GUIContent("Button Scale"));
            script.scale = EditorGUILayout.FloatField("Scale", script.scale);

            if (script.currentType == BTNType.Back)
            {
                EditorGUILayout.PropertyField(m_mainGroup, new GUIContent("Main Group"));
                EditorGUILayout.PropertyField(m_startGroup, new GUIContent("Start Group"));
                EditorGUILayout.PropertyField(m_preview, new GUIContent("Preview"));
                EditorGUILayout.PropertyField(m_stages, new GUIContent("Stages"));
            }
            else if (script.currentType == BTNType.Start)
            {
                EditorGUILayout.PropertyField(m_mainGroup, new GUIContent("Main Group"));
                EditorGUILayout.PropertyField(m_startGroup, new GUIContent("Start Group"));
            }
            else if (script.currentType == BTNType.Stage)
            {
                EditorGUILayout.PropertyField(m_playButton, new GUIContent("Play Button"));
                EditorGUILayout.PropertyField(m_preview, new GUIContent("Preview"));
                EditorGUILayout.PropertyField(m_stage, new GUIContent("Stage"));
                EditorGUILayout.PropertyField(m_stages, new GUIContent("Stages"));
            }
            else if (script.currentType == BTNType.Level)
            {
                script.stageNumber = EditorGUILayout.IntField("Stage Number", script.stageNumber);
                script.levelNumber = EditorGUILayout.IntField("Level Number", script.levelNumber);
                EditorGUILayout.PropertyField(m_playButton, new GUIContent("Play Button"));
                EditorGUILayout.PropertyField(m_previewImage, new GUIContent("Preview Images"));
                EditorGUILayout.PropertyField(m_preview, new GUIContent("Preview"));
            }
            else if (script.currentType == BTNType.Play)
            {
                script.stageNumber = EditorGUILayout.IntField("Stage Number", script.stageNumber);
                script.levelNumber = EditorGUILayout.IntField("Level Number", script.levelNumber);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    public void Start()
    {
        defaultScale = buttonScale.localScale;
    }

    // 버튼 동작 메서드
    public void OnBtnClick()
    {
        switch (currentType)
        {
            case BTNType.Start:
                CanvasGroupOn(startGroup);
                CanvasGroupOff(mainGroup);
                break;
            case BTNType.Exit:
                Application.Quit();
                break;
            case BTNType.Stage:
                InitLevel();
                foreach (var item in stages)
                {
                    CanvasGroupOff(item);
                }
                CanvasGroupOn(stage);
                break;
            case BTNType.Level:
                preview.color = Color.white;
                preview.sprite = previewImage;
                if (playButton.TryGetComponent<Button>(out var btn)) btn.interactable = true;
                if (playButton.TryGetComponent<BtnType>(out var btnType)) btnType.SetStageAndLevel(stageNumber, levelNumber);
                break;
            case BTNType.Back:
                InitStartMenu();
                CanvasGroupOn(mainGroup);
                CanvasGroupOff(startGroup);
                break;
            case BTNType.Play:
                if (stageNumber == 0 || levelNumber == 0) return;
                SceneManager.LoadScene(string.Format("Scenes/Stage_{0}/Level_{1}", stageNumber, levelNumber));
                break;
        }
    }

    public void CanvasGroupOn(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        if (cg.TryGetComponent<Animator>(out var animator) && ContainsParam(animator, "Activated"))
        {
            animator.SetTrigger("Activated");
        }
    }

    public void CanvasGroupOff(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        if (cg.TryGetComponent<Animator>(out var animator) && ContainsParam(animator, "Deactivated"))
        {
            animator.SetTrigger("Deactivated");
        }
    }

    public void InitStartMenu()
    {
        InitLevel();

        foreach (var stage in stages)
        {
            CanvasGroupOff(stage);
        }
    }

    public void InitLevel()
    {
        preview.color = Color.gray;
        preview.sprite = null;

        if (playButton.TryGetComponent<Button>(out var btn)) btn.interactable = false;
        if (playButton.TryGetComponent<BtnType>(out var btnType)) btnType.SetStageAndLevel(0, 0);
    }

    public static bool ContainsParam(Animator _Anim, string _ParamName)
    {
        foreach (AnimatorControllerParameter param in _Anim.parameters)
        {
            if (param.name == _ParamName) return true;
        }
        return false;
    }

    public void SetStageAndLevel(int stageNum, int levelNum)
    {
        stageNumber = stageNum;
        levelNumber = levelNum;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale * scale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }
}
