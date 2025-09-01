using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class levelRecord : MonoBehaviour
{
    private List<string> dataLines = new List<string>();
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "z_axis_angle_and_height_data.csv");
        dataLines.Add("Time,Angle,Height");
    }

    void Update()
    {
        // 获取立方体的z方向向量
        Vector3 cubeZ = transform.forward;

        // 将z方向向量投影到水平面上
        Vector3 zProjectedOnHorizontal = Vector3.ProjectOnPlane(cubeZ, Vector3.up);

        // 计算z轴与水平面的夹角
        float angle = Vector3.Angle(cubeZ, zProjectedOnHorizontal);

        // 获取物体的高度（即y坐标）
        float height = transform.position.y;

        // 记录当前时间、夹角和高度
        string line = Time.time.ToString("F2") + "," + angle.ToString("F2") + "," + height.ToString("F2");
        dataLines.Add(line);

        //Debug.Log("立方体局部z轴与水平面之间的夹角: " + angle + "度, 高度: " + height);
    }

    void OnApplicationQuit()
    {
        File.WriteAllLines(filePath, dataLines);
        Debug.Log("数据已保存到: " + filePath);
    }
}
