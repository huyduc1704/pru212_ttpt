using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using Oink.UIAnimation.Static;

namespace Oink.UIAnimation
{
    [Serializable]
    [CreateAssetMenu(fileName = "OinkAnimation", menuName = "Oink Animation/Animation Unit", order = 1)]
    public class AnimationUnit : ScriptableObject
    {
        [SerializeField]
        private float m_Duration = 1;

        [SerializeField]
        private List<AnimationKey> m_AnimationKeys;
        internal float duration;

        // public properties
        public float Duration { get => m_Duration; set => m_Duration = value; }
        public List<AnimationKey> AnimationKeys { get => m_AnimationKeys; set => m_AnimationKeys = value; }
#if UNITY_EDITOR
        void OnValidate()
        {
            // check key frame
            var count = m_AnimationKeys.Count;
            if (count >= 2)
            {
                for (int i = count - 1; i > 0; i--)
                {
                    AnimationKey endKey = m_AnimationKeys[i];
                    AnimationKey lastKey = m_AnimationKeys[i - 1];
                    endKey.KeyFrameValue = endKey.KeyFrameValue <= lastKey.KeyFrameValue ? lastKey.KeyFrameValue : endKey.KeyFrameValue;
                    m_AnimationKeys[i] = endKey;
                }
            }
        }
    }
#endif

    [Serializable]
    public struct AnimationNode
    {
        [SerializeField]
        string m_Name;

        [SerializeField]
        private OKFeature m_Feature;

        [SerializeField]
        private float m_Value;

        [SerializeField]
        private bool m_UseOriginalValue;

        [SerializeField]
        private Sprite m_Image;

        [SerializeField]
        Settings.OKEasing m_Easing;

        [HideInInspector]
        public bool hideEasing;

        // public properties
        public string Name { readonly get => m_Name; set => m_Name = value; }
        public OKFeature Feature { readonly get => m_Feature; set => m_Feature = value; }
        public float Value { readonly get => m_Value; set => m_Value = value; }
        public bool UseOriginalValue { readonly get => m_UseOriginalValue; set => m_UseOriginalValue = value; }
        public Sprite SpriteImage { readonly get => m_Image; set => m_Image = value; }
        public Settings.OKEasing Easing { readonly get => m_Easing; set => m_Easing = value; }
        public AnimationNode(OKFeature feature = OKFeature.ColorA, short value = 0, bool useOriginalValue = false, Settings.OKEasing easing = Settings.OKEasing.Linear, Sprite spriteImage = null)
        {
            m_Name = "Node name";
            m_Feature = feature;
            m_Value = value;
            m_UseOriginalValue = useOriginalValue;
            m_Easing = easing;
            m_Image = spriteImage;
            hideEasing = false;
        }
    }

    [Serializable]
    public struct AnimationKey
    {
        [SerializeField]
        string m_Name;

        [Range(0, 1)]
        [SerializeField]
        private float m_KeyFrame;

        [SerializeField]
        private List<AnimationNode> m_AnimationNodes;

        // public properties
        public string Name { readonly get => m_Name; set => m_Name = value; }
        public float KeyFrameValue { readonly get => m_KeyFrame; set => m_KeyFrame = value; }
        public List<AnimationNode> AnimationNodes { readonly get => m_AnimationNodes; set => m_AnimationNodes = value; }
        public AnimationKey(List<AnimationNode> animationNodes, float keyFrame = 0)
        {
            m_Name = "Key name";
            m_KeyFrame = keyFrame;
            m_AnimationNodes = animationNodes;
        }
    }

    [Serializable]
    public class AnimationUnitDisplay
    {
        [SerializeField]
        private string m_Name;

        [SerializeField]
        private AnimationUnit m_Unit;

        [SerializeField]
        private bool m_Loop = false;

        [SerializeField]
        private bool m_AlphaTransmit = true;

        [SerializeField]
        private bool m_CanTriggerEvent = true;

        [SerializeField]
        private UnityEvent m_StartEvent;

        [Space(4)]
        [SerializeField]
        private UnityEvent m_CompleteEvent;

        public string Name { get => m_Name; set => m_Name = value; }
        public AnimationUnit Unit { get => m_Unit; set => m_Unit = value; }
        public bool PlayOnLoop { get => m_Loop; set => m_Loop = value; }
        public bool AlphaTransmit { get => m_AlphaTransmit; set => m_AlphaTransmit = value; }
        public bool CanTriggerEvent
        {
            get => m_CanTriggerEvent; set
            {
                if (m_CanTriggerEvent != value)
                {
                    m_CanTriggerEvent = value;
                    if (!m_CanTriggerEvent)
                    {
                        m_StartEvent.RemoveAllListeners();
                        m_CompleteEvent.RemoveAllListeners();
                    }
                }
            }
        }
        public UnityEvent StartEvent { get => m_StartEvent; set => m_StartEvent = value; }
        public UnityEvent CompleteEvent { get => m_CompleteEvent; set => m_CompleteEvent = value; }

        public List<float> ProgressList { get; set; }
        public List<TaskUnit> TaskList { get; set; }
        public float DurationMain { get; set; }
        public bool IsRunning { get; set; }
        public bool IsActive { get; set; }


        public override string ToString()
        {
            return m_Name;
        }

        public void Setup()
        {
            if (!m_CanTriggerEvent)
            {
                m_StartEvent = null;
                m_CompleteEvent = null;
            }
        }

        public void On()
        {
            IsRunning = true;
            IsActive = true;
        }

        public void Off()
        {
            IsRunning = false;
            IsActive = false;
        }
    }

    public struct TaskUnit
    {
        public struct TaskNode
        {
            public Sprite Sprite { get; set; }
            public float StartValue { get; set; }
            public float EndValue { get; }
            public bool IsJump { get; }
            public sbyte StartKeyIndex { get; }
            public sbyte EndKeyIndex { get; }
            public Settings.OKEasing Easing { get; }

            public TaskNode(float endValue) : this(0f, endValue, Settings.OKEasing.Linear, false, null, 0, 0) { }
            public TaskNode(Sprite sprite) : this(0, 0, Settings.OKEasing.Linear, false, sprite, 0, 0) { }
            public TaskNode(float endValue, Settings.OKEasing easing, bool isJump, sbyte startKeyIndex, sbyte endKeyIndex) : this(0f, endValue, easing, isJump, null, startKeyIndex, endKeyIndex) { }
            public TaskNode(float startValue, float endValue, Settings.OKEasing easing, bool isJump, sbyte startKeyIndex, sbyte endKeyIndex) : this(startValue, endValue, easing, isJump, null, startKeyIndex, endKeyIndex) { }
            public TaskNode(float startValue, float endValue, Settings.OKEasing easing, bool isJump, Sprite sprite, sbyte startKeyIndex, sbyte endKeyIndex)
            {
                StartValue = startValue;
                EndValue = endValue;
                Easing = easing;
                IsJump = isJump;
                Sprite = sprite;
                StartKeyIndex = startKeyIndex;
                EndKeyIndex = endKeyIndex;
            }
        }
        public bool IfColor { get; set; }
        public bool IfScale { get; set; }
        public bool IfEulerAngles { get; set; }
        public bool IfPosition { get; set; }
        public bool IfOffset { get; set; }
        public float Duration { get; set; }
        public float EndDuration { get; set; }
        public Dictionary<OKFeature, TaskNode> NodeList { get; set; }

        public TaskUnit(Dictionary<OKFeature, TaskNode> nodeList)
        {
            IfColor = false;
            IfScale = false;
            IfEulerAngles = false;
            IfPosition = false;
            IfOffset = false;
            Duration = 0f;
            EndDuration = 0f;
            NodeList = nodeList;
        }
    }

    public struct UnitData
    {
        public struct KeyInfo
        {
            public float? LastValue { get; set; }
            public float TargetValue { get; set; }
            public Sprite Sprite { get; set; }
            public sbyte StartKeyIndex { get; set; }
            public sbyte EndKeyIndex { get; set; }
            public Settings.OKEasing Easing { get; set; }
            public KeyInfo(float value) : this(null, value, null, 0, 0, 0) { }
            public KeyInfo(Sprite sprite) : this(null, 0, sprite, 0, 0, 0) { }
            public KeyInfo(float value, Settings.OKEasing easing) : this(null, value, null, easing, -1, 0) { }
            public KeyInfo(float value, Settings.OKEasing easing, sbyte startKeyIndex, sbyte endKeyIndex) : this(null, value, null, easing, startKeyIndex, endKeyIndex) { }
            public KeyInfo(float? lastValue, float value, Settings.OKEasing easing, sbyte startKeyIndex, sbyte endKeyIndex) : this(lastValue, value, null, easing, startKeyIndex, endKeyIndex) { }
            public KeyInfo(float? lastValue, float value, Sprite sprite, Settings.OKEasing easing, sbyte startKeyIndex, sbyte endKeyIndex)
            {
                LastValue = lastValue;
                TargetValue = value;
                Sprite = sprite;
                Easing = easing;
                StartKeyIndex = startKeyIndex;
                EndKeyIndex = endKeyIndex;
            }
        }

        public Dictionary<sbyte, KeyInfo> KeyInfos { get; set; }
        public UnitData(bool none = false)
        {
            KeyInfos = null;
        }
        public UnitData(sbyte keyIndex, KeyInfo keyInfo)
        {
            KeyInfos = new Dictionary<sbyte, KeyInfo> { { keyIndex, keyInfo } };
        }
    }

    public enum OKFeature
    {
        Sprite,
        Delay,
        ColorR, ColorG, ColorB, ColorA,
        Scale, ScaleX, ScaleY, ScaleZ,
        RotateX, RotateY, RotateZ,
        PositionX, PositionY, PositionZ,
        OffsetX, OffsetY, OffsetZ,
    }
}