using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine.UI;
using UnityEngine;
using Oink.UIAnimation.Static;
using System.Runtime.Serialization.Formatters;
using Unity.VisualScripting;

namespace Oink.UIAnimation
{
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Oink Animation/OinkAnimation")]
    public class OinkAnimations : MonoBehaviour
    {
        [SerializeField]
        private List<AnimationUnitDisplay> m_AnimationUnitList;

        [SerializeField]
        private bool m_AnimationsCanOverlap;

        [SerializeField]
        private GameObject m_TargetObject;

        private readonly OrderedDictionary m_UnitDisplayTable = new();
        private Transform m_Transform;
        private Image m_Image;
        private Image[] m_ImageComponents = null;
        private bool m_AnimationIsRunning = false;
        private bool delayFinished = true;
        private Coroutine delayCoroutine;
        private int currentIndex = -1;

        void Start()
        {
            if (m_TargetObject == null)
            {
                m_Transform = transform;
                m_Image = GetComponent<Image>();
            }
            else
            {
                if (TryGetComponent<Image>(out Image _image))
                {
                    m_Image = _image;
                    m_Transform = m_TargetObject.transform;
                }
                else
                {
                    Debug.LogError($"no image component found in {name}");
                }
            }

            if (m_AnimationUnitList.Count == 0)
            {
                enabled = false;
                return;
            }
            else
            {
                bool alphaTransmit = false;
                foreach (AnimationUnitDisplay unitDisplay in m_AnimationUnitList)
                {
                    int m_KeySize;
                    float m_DurationMain;
                    Dictionary<OKFeature, UnitData> m_UnitDataTable;
                    List<float> m_ProgessList;
                    List<TaskUnit> m_TaskQueue;

                    (m_KeySize, m_DurationMain, m_UnitDataTable, m_ProgessList) = ReadUnit(unitDisplay.Unit, m_Transform, m_Image.color);
                    m_TaskQueue = GenerateTaskQueue(m_KeySize, m_DurationMain, m_UnitDataTable, m_ProgessList, m_Transform, m_Image.color);

                    unitDisplay.ProgressList = m_ProgessList;
                    unitDisplay.TaskList = m_TaskQueue;
                    unitDisplay.DurationMain = m_DurationMain;
                    unitDisplay.Setup();
                    m_UnitDisplayTable.Add(unitDisplay.ToString(), unitDisplay);

                    alphaTransmit = unitDisplay.AlphaTransmit;
                    if (alphaTransmit) m_ImageComponents = gameObject.GetComponentsInChildren<Image>();
                }
            }
        }

        public void PlayNext(bool isForward = true)
        {
            if (m_UnitDisplayTable.Count == 0)
            {
                Debug.LogWarning($"Animation unit not found!");
                return;
            }
            Stop();

            var keys = m_UnitDisplayTable.Keys.Cast<string>().ToList();
            currentIndex = (isForward ? currentIndex + 1 : (currentIndex - 1 + keys.Count)) % keys.Count;
            string key = keys[currentIndex];
            AnimationUnitDisplay thisTask = (AnimationUnitDisplay)m_UnitDisplayTable[key];

            if (thisTask != null && !thisTask.IsRunning)
            {
                // PrintTaskQueue(thisTask.TaskList);
                thisTask.On();
                m_AnimationIsRunning = true;
                StartCoroutine(TaskProcessor(thisTask));
            }

        }

        public static implicit operator OinkAnimations(Image image)
        {
            return new OinkAnimations() { m_Image = image };
        }

        public void PlaySolo(string _name = "")
        {
            Stop();
            Play(_name);
        }

        public void Play(string _name = "")
        {
            string keyName = _name;
            if (_name == "")
            {
                foreach (string key in m_UnitDisplayTable.Keys)
                {
                    keyName = key;
                    break;
                }
            }

            if (m_UnitDisplayTable.Contains(keyName))
            {
                AnimationUnitDisplay thisTask = m_UnitDisplayTable[keyName] as AnimationUnitDisplay;
                if (thisTask != null && m_AnimationsCanOverlap ? !thisTask.IsRunning : !m_AnimationIsRunning)
                {
                    thisTask.On();
                    m_AnimationIsRunning = true;
                    StartCoroutine(TaskProcessor(thisTask));
                }
                return;
            }

            Debug.LogWarning($"Play(): '{_name} not found!'");
        }

        public void Stop(string _name = "")
        {
            StopProcessor(_name, false);
        }

        public void StopLoop(string _name = "")
        {
            StopProcessor(_name, true);
        }

        private void StopProcessor(string _name, bool isLoop)
        {
            if (_name == "")
            {
                foreach (string key in m_UnitDisplayTable.Keys)
                {
                    if (m_UnitDisplayTable[key] is AnimationUnitDisplay thisTask)
                    {
                        if (isLoop) thisTask.IsActive = false;
                        else thisTask.IsRunning = false;

                        m_AnimationIsRunning = false;
                        StartCoroutine(TaskProcessor(thisTask));
                        break;
                    }
                }
                StopAllCoroutines();
                return;
            }

            if (m_UnitDisplayTable.Contains(_name))
            {
                if (m_UnitDisplayTable[_name] is AnimationUnitDisplay thisTask)
                {
                    if (isLoop) thisTask.IsActive = false;
                    else thisTask.IsRunning = false;

                    m_AnimationIsRunning = IsAnimationRunning();
                    StartCoroutine(TaskProcessor(thisTask));
                    return;
                }
            }

            Debug.LogWarning($"Stop(): '{_name} not found!'");
        }

        public bool IsAnimationRunning()
        {
            bool isRunning = false;
            foreach (string key in m_UnitDisplayTable.Keys)
            {
                if (m_UnitDisplayTable[key] is AnimationUnitDisplay unitDisplay && unitDisplay.IsRunning)
                {
                    isRunning = true;
                    break;
                }
            }
            return isRunning;
        }

        public void Delay(float _value)
        {
            if (delayCoroutine != null)
            {
                StopCoroutine(delayCoroutine);
            }
            delayFinished = false;
            delayCoroutine = StartCoroutine(DelayCoroutine(_value));
        }

        private IEnumerator DelayCoroutine(float _value)
        {
            yield return new WaitForSeconds(_value);
            delayFinished = true;
        }

        private IEnumerator TaskProcessor(AnimationUnitDisplay _unitDisplay)
        {
            if (!_unitDisplay.IsActive) yield break;

            bool canTriggerEvent = _unitDisplay.CanTriggerEvent;
            if (canTriggerEvent)
            {
                _unitDisplay.StartEvent?.Invoke();
                while (!delayFinished) yield return null;
            }

            List<TaskUnit> taskQueue = _unitDisplay.TaskList;
            List<float> progressList = _unitDisplay.ProgressList;
            float durationMain = _unitDisplay.DurationMain;
            bool ifLoop = _unitDisplay.PlayOnLoop;
            bool alphaTransmit = _unitDisplay.AlphaTransmit;
            float elapsedTimeTotal = 0f;
            Vector3 initPos = m_Transform.localPosition;

            for (var i = 0; i < taskQueue.Count; i++)
            {
                if (!_unitDisplay.IsRunning) yield break;

                TaskUnit taskUnit = taskQueue[i];
                Dictionary<OKFeature, TaskUnit.TaskNode> taskList = taskUnit.NodeList;

                float elapsedTime = 0f;
                float duration = taskUnit.Duration;

                Color color = m_Image.color;
                Vector3 scale = m_Transform.localScale;
                Vector3 rotate = m_Transform.localEulerAngles;
                Vector3 postion = m_Transform.localPosition;
                bool hasDelay = false;
                bool hasSprite = false;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;

                    foreach (KeyValuePair<OKFeature, TaskUnit.TaskNode> item in taskList)
                    {
                        TaskUnit.TaskNode taskNode = item.Value;
                        OKFeature key = item.Key;
                        float startValue = taskNode.StartValue;
                        float endValue = taskNode.EndValue;
                        sbyte startKeyIndex = taskNode.StartKeyIndex;
                        sbyte endKeyIndex = taskNode.EndKeyIndex;
                        bool isJump = taskNode.IsJump;
                        float progess;
                        Settings.OKEasing easing = taskNode.Easing;


                        if (!hasDelay && key == OKFeature.Delay)
                        {
                            hasDelay = true;
                            yield return new WaitForSeconds(endValue);
                            continue;
                        }
                        else if (!hasSprite && key == OKFeature.Sprite)
                        {
                            hasSprite = true;
                            m_Image.sprite = taskNode.Sprite;
                            continue;
                        }

                        if (isJump)
                        {
                            float startKeyProgess = startKeyIndex == 0 ? 0 : progressList[startKeyIndex];
                            float endKeyProgess = progressList[endKeyIndex];
                            float gapProgress = endKeyProgess - startKeyProgess;
                            float pastProgress = (elapsedTimeTotal + elapsedTime) / durationMain;
                            progess = (pastProgress - startKeyProgess) / gapProgress;
                        }
                        else { progess = elapsedTime / duration; }

                        float value = Settings.GetAnimatedValue(startValue, endValue, progess, easing);
                        switch (key)
                        {
                            case OKFeature.ColorR: color.r = value; break;
                            case OKFeature.ColorG: color.g = value; break;
                            case OKFeature.ColorB: color.b = value; break;
                            case OKFeature.ColorA: color.a = value; break;
                            case OKFeature.ScaleX: scale.x = value; break;
                            case OKFeature.ScaleY: scale.y = value; break;
                            case OKFeature.ScaleZ: scale.z = value; break;
                            case OKFeature.RotateX: rotate.x = value; break;
                            case OKFeature.RotateY: rotate.y = value; break;
                            case OKFeature.RotateZ: rotate.z = value; break;
                            case OKFeature.PositionX: postion.x = value; break;
                            case OKFeature.PositionY: postion.y = value; break;
                            case OKFeature.PositionZ: postion.z = value; break;
                            case OKFeature.OffsetX: postion.x = initPos.x + value; break;
                            case OKFeature.OffsetY: postion.y = initPos.y + value; break;
                            case OKFeature.OffsetZ: postion.z = initPos.z + value; break;
                        }
                    }

                    if (taskUnit.IfScale) m_Transform.localScale = scale;
                    if (taskUnit.IfEulerAngles) m_Transform.localEulerAngles = rotate;
                    if (taskUnit.IfPosition) m_Transform.localPosition = postion;
                    if (taskUnit.IfOffset) m_Transform.localPosition = postion;
                    if (taskUnit.IfColor)
                    {
                        m_Image.color = color;
                        if (alphaTransmit) foreach (Image image in m_ImageComponents) image.color = color;
                    }

                    yield return null;
                }
                elapsedTimeTotal += duration;
            }

            if (ifLoop) yield return StartCoroutine(TaskProcessor(_unitDisplay));
            _unitDisplay.Off();
            m_AnimationIsRunning = IsAnimationRunning();

            yield return null;
            if (canTriggerEvent) _unitDisplay.CompleteEvent?.Invoke();
        }

        private static List<TaskUnit> GenerateTaskQueue(int _keySize, float _durationMain, Dictionary<OKFeature, UnitData> _unitData, List<float> _progressList, Transform _transform, Color _color)
        {
            List<TaskUnit> result = new();
            TaskUnit taskUnit;
            Dictionary<OKFeature, TaskUnit.TaskNode> nodeList;
            float lastProgress = 0f;

            for (sbyte i = 0; i < _keySize; i++)
            {
                float currentProgress = _progressList[i];
                taskUnit = new TaskUnit { Duration = _durationMain * (currentProgress - lastProgress) };
                lastProgress = currentProgress;
                nodeList = new Dictionary<OKFeature, TaskUnit.TaskNode>();

                foreach (KeyValuePair<OKFeature, UnitData> kvp in _unitData)
                {
                    UnitData unitData = kvp.Value;
                    if (!unitData.KeyInfos.ContainsKey(i)) continue;

                    UnitData.KeyInfo keyInfo = unitData.KeyInfos[i];
                    OKFeature key = kvp.Key;

                    if (key == OKFeature.Delay)
                    {
                        nodeList.Add(key, new TaskUnit.TaskNode(keyInfo.TargetValue));
                        continue;
                    }
                    else if (key == OKFeature.Sprite)
                    {
                        nodeList.Add(key, new TaskUnit.TaskNode(keyInfo.Sprite));
                        continue;
                    }

                    float? startValue = keyInfo.LastValue;
                    float targetValue = keyInfo.TargetValue;
                    sbyte startKeyIndex = keyInfo.StartKeyIndex;
                    sbyte endKeyIndex = keyInfo.EndKeyIndex;
                    Settings.OKEasing easing = keyInfo.Easing;

                    bool ifRewriteInit = false;
                    float rewriteValue = 0f;

                    bool ifStartInit = false;
                    float initValue = 0f;
                    float transformValue = 0f;
                    bool ifJump = (endKeyIndex - startKeyIndex) > 1;

                    bool ifColor = false;
                    bool ifScale = false;
                    bool ifEulerAngles = false;
                    bool ifPosition = false;
                    bool ifOffset = false;

                    if (startKeyIndex == -1)
                    {
                        ifStartInit = true;
                        if (kvp.Value.KeyInfos.ContainsKey(-1))
                        {
                            ifRewriteInit = true;
                            rewriteValue = kvp.Value.KeyInfos[-1].TargetValue;
                        }
                        startKeyIndex = 0;
                    }

                    switch (key)
                    {
                        case OKFeature.ColorR:
                            transformValue = _color.r;
                            ifColor = true;
                            break;
                        case OKFeature.ColorG:
                            transformValue = _color.g;
                            ifColor = true;
                            break;
                        case OKFeature.ColorB:
                            transformValue = _color.b;
                            ifColor = true;
                            break;
                        case OKFeature.ColorA:
                            transformValue = _color.a;
                            ifColor = true;
                            break;
                        case OKFeature.ScaleX:
                            transformValue = _transform.localScale.x;
                            ifScale = true;
                            break;
                        case OKFeature.ScaleY:
                            transformValue = _transform.localScale.y;
                            ifScale = true;
                            break;
                        case OKFeature.ScaleZ:
                            transformValue = _transform.localScale.z;
                            ifScale = true;
                            break;
                        case OKFeature.RotateX:
                            transformValue = _transform.eulerAngles.x;
                            ifEulerAngles = true;
                            break;
                        case OKFeature.RotateY:
                            transformValue = _transform.eulerAngles.y;
                            ifEulerAngles = true;
                            break;
                        case OKFeature.RotateZ:
                            transformValue = _transform.eulerAngles.z;
                            ifEulerAngles = true;
                            break;
                        case OKFeature.PositionX:
                            transformValue = _transform.localPosition.x;
                            ifPosition = true;
                            break;
                        case OKFeature.PositionY:
                            transformValue = _transform.localPosition.y;
                            ifPosition = true;
                            break;
                        case OKFeature.PositionZ:
                            transformValue = _transform.localPosition.z;
                            ifPosition = true;
                            break;
                        case OKFeature.OffsetX:
                        case OKFeature.OffsetY:
                        case OKFeature.OffsetZ:
                            transformValue = 0;
                            ifOffset = true;
                            break;
                    }

                    if (ifColor) taskUnit.IfColor = true;
                    if (ifEulerAngles) taskUnit.IfEulerAngles = true;
                    if (ifPosition) taskUnit.IfPosition = true;
                    if (ifOffset) taskUnit.IfOffset = true;
                    if (ifScale) taskUnit.IfScale = true;

                    initValue = ifRewriteInit ? rewriteValue : transformValue;
                    nodeList.Add(key, new TaskUnit.TaskNode(ifStartInit ? initValue : startValue.Value, targetValue, easing, ifJump, startKeyIndex, endKeyIndex));
                }

                taskUnit.NodeList = nodeList;
                result.Add(taskUnit);
            }
            return result;
        }

        private static (int, float, Dictionary<OKFeature, UnitData>, List<float>) ReadUnit(AnimationUnit _unit, Transform _transform, Color _color)
        {
            List<AnimationKey> list = _unit.AnimationKeys;
            int keySize = list.Count;
            List<float> progessList = new();
            float durationMain = _unit.Duration;
            Dictionary<OKFeature, UnitData> result = new();
            byte amount = 0;

            for (sbyte i = 0; i < keySize; i++)
            {
                float keyFrameValue = list[i].KeyFrameValue;
                progessList.Add(keyFrameValue);

                foreach (var node in list[i].AnimationNodes)
                {
                    amount++;
                    OKFeature key = node.Feature;
                    Settings.OKEasing easing = node.Easing;
                    float value = node.Value;
                    bool useOriginalValue = node.UseOriginalValue;

                    bool isScallAll = false;
                    OKFeature[] scallAllFeatures = null;

                    if (useOriginalValue)
                    {
                        switch (key)
                        {
                            case OKFeature.Delay: value = 0; break;
                            case OKFeature.ColorR: value = _color.r; break;
                            case OKFeature.ColorG: value = _color.g; break;
                            case OKFeature.ColorB: value = _color.b; break;
                            case OKFeature.ColorA: value = _color.a; break;
                            case OKFeature.ScaleX: value = _transform.localScale.x; break;
                            case OKFeature.ScaleY: value = _transform.localScale.y; break;
                            case OKFeature.ScaleZ: value = _transform.localScale.z; break;
                            case OKFeature.RotateX: value = _transform.eulerAngles.x; break;
                            case OKFeature.RotateY: value = _transform.eulerAngles.y; break;
                            case OKFeature.RotateZ: value = _transform.eulerAngles.z; break;
                            case OKFeature.PositionX: value = _transform.localPosition.x; break;
                            case OKFeature.PositionY: value = _transform.localPosition.y; break;
                            case OKFeature.PositionZ: value = _transform.localPosition.z; break;
                            case OKFeature.OffsetX:
                            case OKFeature.OffsetY:
                            case OKFeature.OffsetZ:
                                value = 0;
                                break;
                        }
                    }

                    if (key == OKFeature.Scale)
                    {
                        isScallAll = true;
                        scallAllFeatures = new OKFeature[] { OKFeature.ScaleX, OKFeature.ScaleY, OKFeature.ScaleZ };
                    }

                    if (!result.ContainsKey(key)) // first keyframe
                    {
                        if (key == OKFeature.Delay)
                        {
                            result.Add(key, new UnitData(i, new UnitData.KeyInfo(value)));
                            continue;
                        }
                        else if (key == OKFeature.Sprite)
                        {
                            result.Add(key, new UnitData(i, new UnitData.KeyInfo(node.SpriteImage)));
                            continue;
                        }

                        if (keyFrameValue < Settings.MinimumKeyFrameValue)
                        {
                            // case Scale
                            if (isScallAll)
                            {
                                result.Add(key, new UnitData());
                                foreach (OKFeature feature in scallAllFeatures)
                                {
                                    if (!result.ContainsKey(feature))
                                        result.Add(feature, new UnitData(-1, new UnitData.KeyInfo(value, easing)));
                                }
                                continue;
                            }

                            // case Normal
                            result.Add(key, new UnitData(-1, new UnitData.KeyInfo(value, easing)));
                            continue;
                        }

                        if (i == 0) // if first keyframe
                        {
                            // case Scale
                            if (isScallAll)
                            {
                                result.Add(key, new UnitData());
                                foreach (OKFeature feature in scallAllFeatures)
                                {
                                    if (!result.ContainsKey(feature))
                                    {
                                        result.Add(feature, new UnitData(0, new UnitData.KeyInfo(value, easing, -1, 0)));
                                    }
                                }
                                continue;
                            }

                            // case Normal
                            result.Add(key, new UnitData(0, new UnitData.KeyInfo(value, easing, -1, 0)));
                        }
                        else
                        {
                            if (key == OKFeature.Delay)
                            {
                                result[key].KeyInfos.Add(i, new UnitData.KeyInfo(value));
                                continue;
                            }
                            else if (key == OKFeature.Sprite)
                            {
                                result[key].KeyInfos.Add(i, new UnitData.KeyInfo(node.SpriteImage));
                                continue;
                            }

                            // case Scale
                            if (isScallAll)
                            {
                                result.Add(key, new UnitData());
                                foreach (OKFeature feature in scallAllFeatures)
                                {
                                    if (!result.ContainsKey(feature))
                                    {
                                        result.Add(feature, new UnitData(i, new UnitData.KeyInfo(value, easing, -1, i)));
                                        for (sbyte k = 0; k < i; k++)
                                        {
                                            result[feature].KeyInfos.Add(k, new UnitData.KeyInfo(value, easing, -1, i));
                                        }
                                    }
                                }
                                continue;
                            }

                            // case Normal
                            result.Add(key, new UnitData(i, new UnitData.KeyInfo(value, easing, -1, i)));
                            for (sbyte k = 0; k < i; k++)
                            {
                                result[key].KeyInfos.Add(k, new UnitData.KeyInfo(value, easing, -1, i));
                            }
                        }

                        continue;
                    }

                    // other keyframe
                    // case Scale
                    if (isScallAll)
                    {
                        foreach (OKFeature feature in scallAllFeatures)
                        {
                            UnitData unitScale = result[feature];
                            sbyte a = i;
                            while (!unitScale.KeyInfos.ContainsKey(a)) a--;

                            if (a == -1)
                            {
                                for (sbyte k = 0; k <= i; k++) unitScale.KeyInfos.Add(k, new UnitData.KeyInfo(value, easing, -1, i));
                            }
                            else if (a == 0)
                            {
                                a += 1;
                                float startValue = unitScale.KeyInfos[0].TargetValue;
                                for (sbyte k = a; k <= i; k++) unitScale.KeyInfos.Add(k, new UnitData.KeyInfo(startValue, value, easing, 0, i));
                            }
                            else
                            {
                                float startValue = unitScale.KeyInfos[a].TargetValue;
                                // unitScale.KeyInfos.Add(i, new UnitData.KeyInfo(startValue, value, easing, a, i));
                                for (sbyte k = (sbyte)(a + 1); k <= i; k++) unitScale.KeyInfos.Add(k, new UnitData.KeyInfo(startValue, value, easing, a, i));
                            }
                        }
                        continue;
                    }

                    // case Normal
                    UnitData unitData = result[key];
                    sbyte j = i;
                    while (!unitData.KeyInfos.ContainsKey(j)) j--;

                    if (j == -1)
                    {
                        for (sbyte k = 0; k <= i; k++) unitData.KeyInfos.Add(k, new UnitData.KeyInfo(value, easing, -1, i));
                    }
                    else if (j == 0)
                    {
                        j += 1;
                        float startValue = unitData.KeyInfos[0].TargetValue;
                        for (sbyte k = j; k <= i; k++) unitData.KeyInfos.Add(k, new UnitData.KeyInfo(startValue, value, easing, 0, i));
                    }
                    else
                    {
                        float startValue = unitData.KeyInfos[j].TargetValue;
                        // unitData.KeyInfos[i] = new UnitData.KeyInfo(startValue, value, easing, j, i);
                        for (sbyte k = (sbyte)(j + 1); k <= i; k++) unitData.KeyInfos.Add(k, new UnitData.KeyInfo(startValue, value, easing, j, i));
                    }

                }
            }

            result.Remove(OKFeature.Scale); // clearence
            return amount == 0 ? (0, durationMain, null, progessList) : (keySize, durationMain, result, progessList);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            // check name
            if (m_AnimationUnitList != null && m_AnimationUnitList.Count > 0)
            {
                Dictionary<string, int> nameOccurrences = new();

                for (int i = 0; i < m_AnimationUnitList.Count; i++)
                {
                    AnimationUnitDisplay unit = m_AnimationUnitList[i];
                    string originalName = unit.Name;

                    if (nameOccurrences.ContainsKey(originalName))
                    {
                        nameOccurrences[originalName]++;
                        unit.Name = $"{originalName}_{nameOccurrences[originalName]}";
                    }
                    else
                    {
                        nameOccurrences[originalName] = 0;
                    }
                }
            }
        }

        static void PrintTaskUnit(TaskUnit taskUnit)
        {
            print("TaskUnit Information:");
            print($"Color: {taskUnit.IfColor}" + $" Scale: {taskUnit.IfScale}" + $"; Angles: {taskUnit.IfEulerAngles}" + $"; Position: {taskUnit.IfPosition}");
            print($"Duration: {taskUnit.Duration}" + $"; EndDuration: {taskUnit.EndDuration}");
            print("NodeList Information:");
            foreach (var nodeEntry in taskUnit.NodeList)
            {
                OKFeature feature = nodeEntry.Key;
                TaskUnit.TaskNode node = nodeEntry.Value;

                print($"Feature: {feature}" + $"; StartValue: {node.StartValue}" + $"; EndValue: {node.EndValue}" + $"; IsJump: {node.IsJump}" + $"; StartKeyIndex: {node.StartKeyIndex}" + $"; EndKeyIndex: {node.EndKeyIndex}" + $"; Easing: {node.Easing}");
            }
        }

        static void PrintTaskQueue(List<TaskUnit> taskQueue)
        {
            print("TaskQueue Information:");
            for (int i = 0; i < taskQueue.Count; i++)
            {
                print($"\nTaskUnit {i + 1}:");
                PrintTaskUnit(taskQueue[i]);
            }
        }

        static void DebugNull<T>(string name, T item)
        {
            string output = item == null ? "null" : item.ToString();
            print(name + ": " + output);
        }

        static void PrintList<T>(List<T> list)
        {
            print("List items:");

            foreach (T item in list)
            {
                print(item);
            }
        }
    }
#endif
}
