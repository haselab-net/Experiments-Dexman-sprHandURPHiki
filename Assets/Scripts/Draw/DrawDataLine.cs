using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawDataLine : MonoBehaviour
{
    public LineRenderer lineRenderer;

    //vectorQueue.Enqueue(new Vector3(Random.Range(0, 100), 0, 0));用法示例
    //vectorQueue.Enqueue(new Vector3(Input.mousePosition.x, 0, 0));//用法示例
    static int pointCount = 200;//点数 只能在这里修改
    public float x_scale = 1, y_scale = 1, xPos = 0, yPos = 0, zPos = 0;

    // usage
    Queue vectorQueue = new Queue();
    public Vector3 waitingVector;

    

    void Start() {
        
    }
    float timeStep = 0;
    void FixedUpdate(){

        if(vectorQueue.ToArray().Length >= pointCount)
            vectorQueue.Dequeue();//如果超长度了就踢出
        
        vectorQueue.Enqueue(waitingVector);
        // print(Input.mousePosition.x);
        lineRenderer.positionCount = pointCount;
        for(int i = 0;i < vectorQueue.ToArray().Length;i++){
            var renderVector = (Vector3)vectorQueue.ToArray()[i];
            lineRenderer.SetPosition(i, new Vector3((float)i * x_scale + xPos, renderVector.x * y_scale + yPos, zPos) );
        }
    }


}



