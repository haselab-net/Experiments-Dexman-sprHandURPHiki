using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//也是Offset 临时使用
public class LeapMatrix : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject pinchSphere, leapMotionController;
    Vector4 row1, row2, row3;
    public string HandOffsetMatrixStr = "";//Matlab导出的字符串矩阵
    void Start()
    {
        row1 = new Vector4(3.39510035f, 0.40054097f,  0.04689637f,  0.48797858f);
        row2 = new Vector4(-1.1667273f,   5.80975367f,  0.01968138f, 12.2686027f);
        row3 = new Vector4(-0.22938346f, -2.37813192f, -0.31384057f, -0.44098878f);
    }

    // Update is called once per frame
    void Update()
    {
        var meshr = pinchSphere.GetComponent<MeshRenderer>();
        meshr.enabled = false;//让Pinch球消失方便测试
        if(HandOffsetMatrixStr != ""){//使用字符串矩阵
            StrToMat(HandOffsetMatrixStr);
        }
        Vector3 inputCoordinate = new Vector3(pinchSphere.transform.localPosition.x, pinchSphere.transform.localPosition.y, pinchSphere.transform.localPosition.z);
        var result = new Vector3(calc(row1, inputCoordinate), calc(row2, inputCoordinate), calc(row3, inputCoordinate));
        //print(result);
        leapMotionController.transform.position = result;
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

    float calc(Vector4 row, Vector3 input){
        var tmp1 = row[0] * input[0];
        var tmp2 = row[1] * input[1];
        var tmp3 = row[2] * input[2];
        var tmp4 = row[3] * 1;
        return tmp1 + tmp2 + tmp3 + tmp4;
    }
}
