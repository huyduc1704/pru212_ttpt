namespace Oink.UIAnimation
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;
    using UnityEditorInternal;
    using Oink.UIAnimation;
    using Oink.UIAnimation.Static;
    using System.Collections.Generic;

    [CustomEditor(typeof(OinkAnimations))]
    [CanEditMultipleObjects]
    public class OinkAnimationsEditor : Editor
    {
        private ReorderableList animationUnitList;

        private void OnEnable()
        {
            // Initialize the ReorderableList for m_AnimationUnitList
            animationUnitList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("m_AnimationUnitList"),
                true, true, true, true);

            // Customize the element display
            animationUnitList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = animationUnitList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += Settings.GUIPadding;

                float labelWidth = rect.width * Settings.GUILabelWidth;
                float fieldWidth = rect.width * Settings.GUIValueWidth;

                // Track the initial y position for the border
                float initialYPosition = rect.y;

                // Draw m_Name field with label
                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Unit ID");
                EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Name"), GUIContent.none);

                // Move to the next line for m_Unit
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Unit / 文件");
                EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Unit"), GUIContent.none);

                // Move to the next line for m_Loop and m_AlphaTransmit
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Loop / 循环");
                EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Loop"), GUIContent.none);

                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Alpha Transmit / 透明度传递");
                EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_AlphaTransmit"), GUIContent.none);

                // Draw the m_CanTriggerEvent field
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Trigger Events / 触发事件");
                EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_CanTriggerEvent"), GUIContent.none);

                // Conditionally draw the m_StartEvent and m_CompleteEvent fields
                if (element.FindPropertyRelative("m_CanTriggerEvent").boolValue)
                {
                    rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Events", EditorStyles.boldLabel);

                    rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(element.FindPropertyRelative("m_StartEvent"), true)), element.FindPropertyRelative("m_StartEvent"), true);

                    rect.y += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("m_StartEvent"), true) + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(element.FindPropertyRelative("m_CompleteEvent"), true)), element.FindPropertyRelative("m_CompleteEvent"), true);
                }
            };

            // Customize the header
            animationUnitList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Animation Units");
            };

            // Handle element height
            animationUnitList.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = animationUnitList.serializedProperty.GetArrayElementAtIndex(index);

                float height = 4 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

                if (element.FindPropertyRelative("m_CanTriggerEvent").boolValue)
                {
                    height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    height += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("m_StartEvent"), true) + EditorGUIUtility.standardVerticalSpacing;
                    height += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("m_CompleteEvent"), true) + EditorGUIUtility.standardVerticalSpacing;
                }

                height += EditorGUIUtility.singleLineHeight + 10f; // Adding space for the bottom border

                return height;
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Animations / 动画", EditorStyles.boldLabel);
            EditorGUILayout.Space(6);
            animationUnitList.DoLayoutList();

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Setup / 设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            SerializedProperty overlayProperty = serializedObject.FindProperty("m_AnimationsCanOverlap");
            EditorGUILayout.PropertyField(overlayProperty, new GUIContent("Can Overlap / 允许重叠"));

            SerializedProperty objectProperty = serializedObject.FindProperty("m_TargetObject");
            EditorGUILayout.PropertyField(objectProperty, new GUIContent("Target Object / 目标物体"));

            // EditorGUILayout.PropertyField(serializedObject.FindProperty("m_AnimationsCanOverlap"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("m_GameObject"));

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(AnimationUnit))]
    [CanEditMultipleObjects]
    public class AnimationUnitEditor : Editor
    {
        private ReorderableList animationKeyList;

        private void OnEnable()
        {
            serializedObject.Update();

            // Initialize the ReorderableList for m_AnimationKeys
            animationKeyList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("m_AnimationKeys"),
                true, true, true, true);

            // Customize the element display using PropertyDrawer (AnimationKeyDrawer)
            animationKeyList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = animationKeyList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += Settings.GUIPadding;

                // Use EditorGUI.PropertyField to respect the custom drawer (AnimationKeyDrawer)
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };

            // Customize the header
            animationKeyList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Animation Keys");
            };

            // Handle adding new elements
            animationKeyList.onAddCallback = (ReorderableList list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;

                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

                element.FindPropertyRelative("m_Name").stringValue = "New Key";
                element.FindPropertyRelative("m_KeyFrame").floatValue = 0;
                element.FindPropertyRelative("m_AnimationNodes").ClearArray();
            };

            // Handle element height
            animationKeyList.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = animationKeyList.serializedProperty.GetArrayElementAtIndex(index);

                // Use GetPropertyHeight to respect custom drawer's height calculation
                return EditorGUI.GetPropertyHeight(element, GUIContent.none, true) + 4; // Added padding
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Settings / 设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(3);
            SerializedProperty durationProperty = serializedObject.FindProperty("m_Duration");
            EditorGUILayout.PropertyField(durationProperty, new GUIContent("Duration / 长度"));
            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField("Keyframes / 关键帧", EditorStyles.boldLabel);
            EditorGUILayout.Space(6);
            animationKeyList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(AnimationKey))]
    public class AnimationKeyDrawer : PropertyDrawer
    {
        private readonly Dictionary<string, ReorderableList> reorderableLists = new Dictionary<string, ReorderableList>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Start property drawing
            EditorGUI.BeginProperty(position, label, property);

            // Get all the properties
            var nameProperty = property.FindPropertyRelative("m_Name");
            var keyFrameProperty = property.FindPropertyRelative("m_KeyFrame");
            var animationNodesProperty = property.FindPropertyRelative("m_AnimationNodes");

            // Generate a unique key for each AnimationKey
            string listKey = property.propertyPath;

            // Initialize the ReorderableList if it's not already initialized for this specific AnimationKey
            if (!reorderableLists.ContainsKey(listKey))
            {
                reorderableLists[listKey] = InitializeReorderableList(animationNodesProperty, keyFrameProperty);
            }

            var animationNodeList = reorderableLists[listKey];

            // Calculate rects
            var singleLineHeight = EditorGUIUtility.singleLineHeight;
            var lineSpacing = EditorGUIUtility.standardVerticalSpacing;
            var padding = Settings.GUIPadding; // Additional padding

            var yOffset = position.y;

            // Define label width
            float labelWidth = position.width * Settings.GUILabelWidth;
            float fieldWidth = position.width * Settings.GUIValueWidth;

            // Rects for labels and fields
            var nameLabelRect = new Rect(position.x, yOffset, labelWidth, singleLineHeight);
            var nameFieldRect = new Rect(position.x + labelWidth, yOffset, fieldWidth, singleLineHeight);
            yOffset += singleLineHeight + lineSpacing + padding;

            var keyFrameLabelRect = new Rect(position.x, yOffset, labelWidth, singleLineHeight);
            var keyFrameFieldRect = new Rect(position.x + labelWidth, yOffset, fieldWidth, singleLineHeight);
            yOffset += singleLineHeight + lineSpacing + padding;

            // Draw fields with labels
            EditorGUI.LabelField(nameLabelRect, "Key Name / 帧名字");
            EditorGUI.PropertyField(nameFieldRect, nameProperty, GUIContent.none);

            EditorGUI.LabelField(keyFrameLabelRect, "Frame Value / 帧值");
            EditorGUI.PropertyField(keyFrameFieldRect, keyFrameProperty, GUIContent.none);

            // Draw the ReorderableList
            var listRect = new Rect(position.x, yOffset, position.width, animationNodeList.GetHeight());
            animationNodeList.DoList(listRect);

            // End property drawing
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var animationNodesProperty = property.FindPropertyRelative("m_AnimationNodes");
            var singleLineHeight = EditorGUIUtility.singleLineHeight;
            var padding = Settings.GUIPadding; // Additional padding

            // Generate a unique key for each AnimationKey
            string listKey = property.propertyPath;

            if (!reorderableLists.ContainsKey(listKey))
            {
                var keyFrameProperty = property.FindPropertyRelative("m_KeyFrame");
                reorderableLists[listKey] = InitializeReorderableList(animationNodesProperty, keyFrameProperty);
            }

            var animationNodeList = reorderableLists[listKey];

            return 2 * (singleLineHeight + padding) + EditorGUIUtility.standardVerticalSpacing + animationNodeList.GetHeight();
        }

        private ReorderableList InitializeReorderableList(SerializedProperty animationNodesProperty, SerializedProperty keyFrameProperty)
        {
            return new ReorderableList(animationNodesProperty.serializedObject, animationNodesProperty, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = animationNodesProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    // Set custom width (e.g., 80% of the available width)
                    float nameWidth = rect.width * Settings.GUINameWidth;
                    float featureWidth = rect.width * Settings.GUIFeatureWidth;
                    float valueWidth = rect.width * Settings.GUIValueWidth;
                    float easingWidth = rect.width * Settings.GUIEasingWidth;
                    float labelWidth = rect.width * Settings.GUILabelWidth;
                    float rightAlignedX = rect.x + labelWidth;

                    // Determine if the easing should be hidden based on the keyFrame value
                    bool hideEasing = keyFrameProperty.floatValue < Settings.MinimumKeyFrameValue;
                    element.FindPropertyRelative("hideEasing").boolValue = hideEasing;

                    // Calculate the height for the current element
                    var singleLineHeight = EditorGUIUtility.singleLineHeight;
                    var lineSpacing = EditorGUIUtility.standardVerticalSpacing;
                    var yOffset = rect.y;

                    var nameProperty = element.FindPropertyRelative("m_Name");
                    var featureProperty = element.FindPropertyRelative("m_Feature");
                    var valueProperty = element.FindPropertyRelative("m_Value");
                    var useOriginalValueProperty = element.FindPropertyRelative("m_UseOriginalValue");
                    var spriteImageProperty = element.FindPropertyRelative("m_Image");
                    var easingProperty = element.FindPropertyRelative("m_Easing");

                    // Draw the Name field and label
                    var nameLabelRect = new Rect(rect.x, yOffset, labelWidth, singleLineHeight);
                    var nameRect = new Rect(rightAlignedX, yOffset, nameWidth, singleLineHeight);
                    EditorGUI.LabelField(nameLabelRect, "Node Name / 节点名字");
                    EditorGUI.PropertyField(nameRect, nameProperty, GUIContent.none);
                    yOffset += singleLineHeight + lineSpacing;

                    // Draw the Feature field and label
                    var featureLabelRect = new Rect(rect.x, yOffset, labelWidth, singleLineHeight);
                    var featureRect = new Rect(rightAlignedX, yOffset, featureWidth, singleLineHeight);
                    EditorGUI.LabelField(featureLabelRect, "Feature / 元素");
                    EditorGUI.PropertyField(featureRect, featureProperty, GUIContent.none);
                    yOffset += singleLineHeight + lineSpacing;

                    // Conditionally draw the Value or SpriteImage field and label
                    if ((OKFeature)featureProperty.enumValueIndex == OKFeature.Sprite)
                    {
                        var spriteLabelRect = new Rect(rect.x, yOffset, labelWidth, singleLineHeight);
                        var spriteRect = new Rect(rightAlignedX, yOffset, valueWidth, singleLineHeight);
                        EditorGUI.LabelField(spriteLabelRect, "Sprite / 图片");
                        EditorGUI.PropertyField(spriteRect, spriteImageProperty, GUIContent.none);
                        yOffset += singleLineHeight + lineSpacing;
                    }
                    else if ((OKFeature)featureProperty.enumValueIndex == OKFeature.Delay)
                    {
                        var valueLabelRect = new Rect(rect.x, yOffset, labelWidth, singleLineHeight);
                        var valueRect = new Rect(rightAlignedX, yOffset, valueWidth, singleLineHeight);
                        EditorGUI.LabelField(valueLabelRect, "Value / 值");
                        EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
                    }
                    else
                    {
                        bool useOriginalValue = useOriginalValueProperty.boolValue;

                        var valueLabelRect = new Rect(rect.x, yOffset, labelWidth, singleLineHeight);
                        var valueRect = new Rect(rightAlignedX, yOffset, valueWidth, singleLineHeight);
                        EditorGUI.LabelField(valueLabelRect, "Value / 值");
                        if (!useOriginalValue) EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
                        yOffset += singleLineHeight + lineSpacing;

                        var useOriginalValueLabelRect = new Rect(rect.x, yOffset, labelWidth, singleLineHeight);
                        var useOriginalValueRect = new Rect(rightAlignedX, yOffset, valueWidth, singleLineHeight);
                        EditorGUI.LabelField(useOriginalValueLabelRect, "Use Original Value / 使用原始值");
                        EditorGUI.PropertyField(useOriginalValueRect, useOriginalValueProperty, GUIContent.none);
                        yOffset += singleLineHeight + lineSpacing;

                        if (!hideEasing)
                        {
                            var easingLabelRect = new Rect(rect.x, yOffset, labelWidth, singleLineHeight);
                            var easingRect = new Rect(rightAlignedX, yOffset, easingWidth, singleLineHeight);
                            EditorGUI.LabelField(easingLabelRect, "Easing / 动画函数");
                            EditorGUI.PropertyField(easingRect, easingProperty, GUIContent.none);
                            yOffset += singleLineHeight + lineSpacing;
                        }
                    }
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Animation Nodes");
                },

                elementHeightCallback = (int index) =>
                {
                    var element = animationNodesProperty.GetArrayElementAtIndex(index);
                    var singleLineHeight = EditorGUIUtility.singleLineHeight;
                    var lineSpacing = EditorGUIUtility.standardVerticalSpacing;

                    // Determine if the easing should be hidden based on the keyFrame value
                    bool hideEasing = keyFrameProperty.floatValue < Settings.MinimumKeyFrameValue;
                    element.FindPropertyRelative("hideEasing").boolValue = hideEasing;

                    // Calculate the height based on whether easing is hidden or not
                    float height = 4 * (singleLineHeight + lineSpacing); // Name, Feature, and either Value or SpriteImage

                    if (!hideEasing)
                    {
                        height += singleLineHeight + lineSpacing; // Add space for Easing
                    }

                    height += 4f;

                    return height;
                },

                onAddCallback = (ReorderableList list) =>
                {
                    // Get the index for the new element
                    var index = animationNodesProperty.arraySize;

                    // Insert a new element in the array at the current index
                    animationNodesProperty.InsertArrayElementAtIndex(index);

                    // Get the newly added element
                    var newElement = animationNodesProperty.GetArrayElementAtIndex(index);

                    bool hideEasing = keyFrameProperty.floatValue < Settings.MinimumKeyFrameValue;
                    newElement.FindPropertyRelative("hideEasing").boolValue = hideEasing;

                    // Ensure that newElement is valid
                    if (newElement != null)
                    {
                        newElement.FindPropertyRelative("m_Name").stringValue = "";
                        newElement.FindPropertyRelative("m_Feature").enumValueIndex = (int)OKFeature.Delay;
                        newElement.FindPropertyRelative("m_Value").floatValue = 0;
                        newElement.FindPropertyRelative("m_UseOriginalValue").boolValue = false;
                        newElement.FindPropertyRelative("m_Easing").enumValueIndex = (int)Settings.OKEasing.Linear;
                    }
                    else
                    {
                        Debug.LogError("Failed to insert new element into animationKeysProperty.");
                    }
                }
            };
        }
    }

#endif
}