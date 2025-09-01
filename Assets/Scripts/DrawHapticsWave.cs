using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawHapticsWave : MonoBehaviour
{
    public LineRenderer lineRenderer;

    //vectorQueue.Enqueue(new Vector3(Random.Range(0, 100), 0, 0));用法示例
    //vectorQueue.Enqueue(new Vector3(Input.mousePosition.x, 0, 0));//用法示例
    static int pointCount = 512;//点数 只能在这里修改
    public float x_scale = 1, y_scale = 1, z_scale = 1, xPos = 0, yPos = 0, zPos = 0;

    // usage
    private float[] audioData;
    public int smoothFactor = 10; // 平滑系数
    public float[] smoothingData; // 存储平滑数据

    void Start() {
        lineRenderer.positionCount = pointCount;
        audioData = new float[pointCount]; // 存储音频数据的数组
        smoothingData = new float[pointCount];
    }
    float timeStep = 0;
    void FixedUpdate(){
        // 读取音频数据
        //AudioListener.GetOutputData(audioData, 0);
        for(int i = 0;i < audioData.Length;i++){
            //smoothingData[i] += (audioData[i] / 2000 - smoothingData[i]) / smoothFactor;
            var renderVector = new Vector3(audioData[i] / 2000, 0, 0);
            //lineRenderer.SetPosition(i, new Vector3((float)i * x_scale + xPos, renderVector.x * y_scale + yPos, zPos) );
            lineRenderer.SetPosition(i, new Vector3((float)i * x_scale + xPos, renderVector.x * y_scale + yPos, zPos) );
        }
    }

    public void AddNewValue(float newNumber)
    {
        // 判断数组是否为空
        if (audioData == null || audioData.Length == 0)
        {
            print("empty array");
        }

            float[] newArray = new float[audioData.Length];

        // 将旧数组中的数据往前移动一位
        for (int i = 0; i < audioData.Length - 1; i++)
        {
            newArray[i] = audioData[i + 1];
        }

        // 将新数字添加到末尾
        newArray[newArray.Length - 1] = newNumber;

        audioData = newArray;
    }




}



