using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

//用于测量手指压力是否变化的
public class DataLogger : MonoBehaviour
{
    private StreamWriter writer;
    private string filePath;

    public string userNum = "2", expNum = "1";

    public static List<float> dataList;

    public GameObject thumbCapsule, indexCapsule, cube;
    public forceHapticSender fh;



    void Start()
    {

        dataList = new List<float>();
        // 创建文件名
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filePath = Application.dataPath + "/" + userNum + "_" + expNum + "_" + timestamp + ".csv";

        // 创建StreamWriter实例
        writer = new StreamWriter(filePath, true);

        // 写入CSV头部（根据你的向量修改列名）
        if(expNum == "2.1")
            writer.WriteLine("rv, rvv, objectZ, distance, angularx, angulary, angularz, stickSlip");
        else
            writer.WriteLine("rv, rvv, objectY, distance, angularx, angulary, angularz, stickSlip, thumbForce");
    }

    

    void FixedUpdate()
    {
        // 获取你的向量数据，这里以transform.position为例
        

        float pinchDistance = Vector3.Distance(thumbCapsule.transform.position, indexCapsule.transform.position);

        dataList.Add(fh.relativeLinV);
        dataList.Add(fh.relativeAngularV);
        if(expNum == "2.1")
            dataList.Add(cube.transform.position.z);
        else
            dataList.Add(cube.transform.position.y);
        dataList.Add(pinchDistance);
        dataList.Add(cube.transform.eulerAngles.x);
        dataList.Add(cube.transform.eulerAngles.y);
        dataList.Add(cube.transform.eulerAngles.z);
        dataList.Add(fh.isStickslip);
        dataList.Add(fh.forceForLog);

        string csvLine = string.Join(",", dataList);
        // 将生成的字符串写入文件
        writer.WriteLine(csvLine);

        // 确保数据实时写入到文件
        writer.Flush();
        dataList.Clear();
    }

    void OnDestroy()
    {
        // 确保在脚本停止时关闭StreamWriter
        if (writer != null)
        {
            writer.Close();
        }
    }
}
