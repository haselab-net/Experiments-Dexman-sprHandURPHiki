using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawTimeSound : MonoBehaviour
{
    public LineRenderer lineRenderer;

    //vectorQueue.Enqueue(new Vector3(Random.Range(0, 100), 0, 0));用法示例
    //vectorQueue.Enqueue(new Vector3(Input.mousePosition.x, 0, 0));//用法示例
    static int pointCount = 512;//点数 只能在这里修改
    public float x_scale = 1, y_scale = 1, z_scale = 1, xPos = 0, yPos = 0, zPos = 0;

    // usage
    private float[] audioData;
    private float[] audioData1;
    public int smoothFactor = 10; // 平滑系数
    public float[] smoothingData; // 存储平滑数据

    void Start() {
        lineRenderer.positionCount = pointCount;
        audioData = new float[pointCount]; // 存储音频数据的数组
        audioData1 = new float[pointCount]; // 存储音频数据的数组
        smoothingData = new float[pointCount];
    }
    float timeStep = 0;
    void FixedUpdate(){

        

        // 读取音频数据
        AudioListener.GetOutputData(audioData, 0);
        AudioListener.GetOutputData(audioData1, 1);
        for(int i = 0;i < audioData.Length;i++){
            smoothingData[i] += ((audioData[i] + audioData1[i]) / 2 - smoothingData[i]) / smoothFactor;
            var renderVector = new Vector3(smoothingData[i], 0, 0);
            //lineRenderer.SetPosition(i, new Vector3((float)i * x_scale + xPos, renderVector.x * y_scale + yPos, zPos) );
            lineRenderer.SetPosition(i, new Vector3((float)i * x_scale + xPos, yPos, renderVector.x * z_scale + zPos) );
        }
    }


}



