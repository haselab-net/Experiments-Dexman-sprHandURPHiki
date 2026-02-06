using UnityEngine;
using UnityEditor;

// Custom inspector for forceHapticSender to manage motorConfigs list
[CustomEditor(typeof(forceHapticSender))]
public class forceHapticSenderEditor : Editor
{
    SerializedProperty motorConfigsProp;

    private void OnEnable()
    {
        motorConfigsProp = serializedObject.FindProperty("motorConfigs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw default properties excluding motorConfigs
        DrawPropertiesExcluding(serializedObject, "motorConfigs");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Motor Configurations", EditorStyles.boldLabel);

        // Draw each motor config with foldout and sub-fields
        for (int i = 0; i < motorConfigsProp.arraySize; i++)
        {
            SerializedProperty element = motorConfigsProp.GetArrayElementAtIndex(i);
            SerializedProperty labelProp = element.FindPropertyRelative("motorLabel");
            SerializedProperty idProp = element.FindPropertyRelative("motorId");
            SerializedProperty enabledProp = element.FindPropertyRelative("isEnabled");

            // Assign default label if empty
            if (string.IsNullOrEmpty(labelProp.stringValue))
            {
                labelProp.stringValue = ((char)('A' + i)).ToString();
            }

            
            // 电机启用/禁用控制（复选框），标题显示包含电机启用状态信息
            EditorGUILayout.BeginHorizontal();
            enabledProp.boolValue = EditorGUILayout.Toggle(enabledProp.boolValue, GUILayout.Width(18));
            
            // 根据启用状态改变标题颜色
            GUI.color = enabledProp.boolValue ? Color.white : new Color(0.7f, 0.7f, 0.7f, 0.8f);
            element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, "Motor " + labelProp.stringValue + " (ID: " + idProp.intValue + ")" + (enabledProp.boolValue ? "" : " - 已禁用"));
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            
            if (element.isExpanded)
            {
                EditorGUI.indentLevel++;
                
                // Basic properties
                EditorGUILayout.PropertyField(labelProp, new GUIContent("Label"));
                EditorGUILayout.PropertyField(idProp, new GUIContent("Motor ID"));
                
                // X轴响应系数
                EditorGUILayout.LabelField("X轴系数", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(element.FindPropertyRelative("kxPos"), new GUIContent("X正向(右)"));
                EditorGUILayout.PropertyField(element.FindPropertyRelative("kxNeg"), new GUIContent("X负向(左)"));
                EditorGUI.indentLevel--;
                
                // Y轴响应系数
                EditorGUILayout.LabelField("Y轴系数", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(element.FindPropertyRelative("kyPos"), new GUIContent("Y正向(远离)"));
                EditorGUILayout.PropertyField(element.FindPropertyRelative("kyNeg"), new GUIContent("Y负向(靠近)"));
                EditorGUI.indentLevel--;
                
                // Z轴响应系数
                EditorGUILayout.LabelField("Z轴系数", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(element.FindPropertyRelative("kzPos"), new GUIContent("Z正向"));
                EditorGUILayout.PropertyField(element.FindPropertyRelative("kzNeg"), new GUIContent("Z负向(压力)"));
                EditorGUI.indentLevel--;
                
                // 输出范围和增益
                EditorGUILayout.LabelField("力输出范围", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(element.FindPropertyRelative("lowerLimit"), new GUIContent("Lower Limit (L)"));
                EditorGUILayout.PropertyField(element.FindPropertyRelative("upperLimit"), new GUIContent("Upper Limit (U)"));
                EditorGUILayout.PropertyField(element.FindPropertyRelative("gain"), new GUIContent("Gain (g)"));
                EditorGUI.indentLevel--;
                
                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Motor"))
        {
            motorConfigsProp.arraySize++;
            var newElem = motorConfigsProp.GetArrayElementAtIndex(motorConfigsProp.arraySize - 1);
            newElem.FindPropertyRelative("motorLabel").stringValue = ((char)('A' + motorConfigsProp.arraySize - 1)).ToString();
            newElem.FindPropertyRelative("motorId").intValue = motorConfigsProp.arraySize;
            
            // 默认值
            newElem.FindPropertyRelative("isEnabled").boolValue = true; // 默认启用
            newElem.FindPropertyRelative("kxPos").floatValue = 1f;
            newElem.FindPropertyRelative("kxNeg").floatValue = 1f;
            newElem.FindPropertyRelative("kyPos").floatValue = 1f;
            newElem.FindPropertyRelative("kyNeg").floatValue = 1f;
            newElem.FindPropertyRelative("kzPos").floatValue = 1f;
            newElem.FindPropertyRelative("kzNeg").floatValue = 1f;
        }
        if (GUILayout.Button("Remove Motor") && motorConfigsProp.arraySize > 1)
        {
            motorConfigsProp.DeleteArrayElementAtIndex(motorConfigsProp.arraySize - 1);
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
} 