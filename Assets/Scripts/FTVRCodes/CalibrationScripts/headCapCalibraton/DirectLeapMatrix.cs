using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectLeapMatrix : MonoBehaviour
{
    // Start is called before the first frame update
    //直接装到指尖球

    
    public Vector4 row1, row2, row3;
    Vector3 HeadOffsetVector;//眼睛和帽子指尖的offset

    public string MatrixStr = "";//Matlab导出的字符串矩阵
    public bool isLeftHand = false;
    Vector3 inputCoordinate;
    void Start()
    {
        if(MatrixStr != ""){//使用字符串矩阵
            
            StrToMat(MatrixStr);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(MatrixStr != ""){//使用字符串矩阵
            
            StrToMat(MatrixStr);
        }
        if(isLeftHand == true)
            inputCoordinate = LeapPinch.leftPinchPositin;
        else
            inputCoordinate = LeapPinch.pinchPosition;
        var result = new Vector3(calc(row1, inputCoordinate), calc(row2, inputCoordinate), calc(row3, inputCoordinate));
        //result += HeadOffsetVector;
        transform.position = result;
        //transform.rotation = Quaternion.Euler(new Vector3(UDP.cam_Rotatex, UDP.cam_Rotatey, UDP.cam_Rotatez));
        
    }
    
    void StrToMat(string str){//输入字符串修改矩阵
        var OutputMatrix =  str.Split(',');
        List<Vector4>  resultMatrix = new List<Vector4>();
        List<float>  resultFloatList = new List<float>();//最终结果
        for(int i = 0;i < OutputMatrix.Length;i++){//把字符串转成float塞进去
            resultFloatList.Add(float.Parse(OutputMatrix[i]));
        }
        row1 = new Vector4(resultFloatList[0], resultFloatList[1], resultFloatList[2], resultFloatList[3]);
        row2 = new Vector4(resultFloatList[4], resultFloatList[5], resultFloatList[6], resultFloatList[7]);
        row3 = new Vector4(resultFloatList[8], resultFloatList[9], resultFloatList[10], resultFloatList[11]);
    }
    public float calc(Vector4 row, Vector3 input){
        var tmp1 = row[0] * input[0];
        var tmp2 = row[1] * input[1];
        var tmp3 = row[2] * input[2];
        var tmp4 = row[3] * 1;
        return tmp1 + tmp2 + tmp3 + tmp4;
    }
}
