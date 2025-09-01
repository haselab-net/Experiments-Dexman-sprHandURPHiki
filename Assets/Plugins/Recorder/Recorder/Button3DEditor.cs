using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Button3D))]
public class Button3DEditor : Editor
{
    string tagFilter = "";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 绘制默认的Inspector

        Button3D script = (Button3D)target;

        // 添加一个文本框以输入标签
        tagFilter = EditorGUILayout.TextField("Tag Filter", tagFilter);

        if (GUILayout.Button("Add Tagged Objects to Record"))
        {
            //script.objectsToRecord.Clear(); // 清除当前列表
            foreach (var gameObject in GameObject.FindGameObjectsWithTag(tagFilter))
            {
                // 添加具有指定标签的对象
                script.objectsToRecord.Add(gameObject);
            }
        }

        if (GUILayout.Button("Add all Objects to Record"))
        {
            //script.objectsToRecord.Clear(); // 清除当前列表
            foreach (var meshObject in FindObjectsOfType<MeshFilter>())
            {
                // 添加具有MeshFilter组件的对象
                script.objectsToRecord.Add(meshObject.gameObject);
            }
        }
    }
}
