using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixCalculation : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector4 row1, row2, row3;
    Vector3 HeadOffsetVector;//眼睛和帽子指尖的offset

    public string MatrixStr = "";//Matlab导出的字符串矩阵
    void Start()
    {
        
        //
//    
//    
//   
//    
//    
//     
//    
//          0
//          0
//          0
//    -4.0000

        // row1 = new Vector4(-23.3257f, 0.9249f,-9.9413f,-21.7634f);
        // row2 = new Vector4(-0.1327f,24.0579f,2.7187f,14.8889f);
        // row3 = new Vector4(0, 0, 0, -4);

        // row1 = new Vector4(-23.122507345406714f,0.39774237718376027f,-4.696394801183594f,-12.18071942771724f);
        // row2 = new Vector4(0.2095963897498313f,19.91756063888694f,-0.7335290649336592f,9.12595643327129f);
        // row3 = new Vector4(-7.582452590958021f,0.3780691642998488f,25.464567386541734f,44.09622368044489f);

        Vector3 UpperTracker = new Vector3(0.0195f, 0.3255f, -1.9565f);//Y更大的那个
        Vector3 LowerTracker = new Vector3(0.0270f, 0.2023f, -2.0907f);//
        UpperTracker = new Vector3(calc(row1, UpperTracker), calc(row2, UpperTracker), calc(row3, UpperTracker));
        LowerTracker = new Vector3(calc(row1, LowerTracker), calc(row2, LowerTracker), calc(row3, LowerTracker));
        HeadOffsetVector = UpperTracker - LowerTracker;
        if(MatrixStr != ""){//使用字符串矩阵
            
            StrToMat(MatrixStr);
        }
        //print(new Vector3(calc(row1, inputCoordinate), calc(row2, inputCoordinate), calc(row3, inputCoordinate)));
        var a1 = new Vector3(calc(row1, new Vector3(1, 0, 0)), calc(row2, new Vector3(1, 0, 0)), calc(row3, new Vector3(1, 0, 0)));
        var a2 =new Vector3(calc(row1, new Vector3(-1, 0, 0)), calc(row2, new Vector3(-1, 0, 0)), calc(row3, new Vector3(-1, 0, 0)));

        //print(Vector3.Distance(a1, a2) / 2f);

        var b1 =(new Vector3(calc(row1, new Vector3(0, 1, 0)), calc(row2, new Vector3(0, 1, 0)), calc(row3, new Vector3(0, 1, 0))));
        var b2 =(new Vector3(calc(row1, new Vector3(0, -1, 0)), calc(row2, new Vector3(0, -1, 0)), calc(row3, new Vector3(0, -1, 0))));

        //print(Vector3.Distance(b1, b2) / 2);

        var c1 =new Vector3(calc(row1, new Vector3(0, 0, 1)), calc(row2, new Vector3(0, 0, 1)), calc(row3, new Vector3(0, 0, 1)));
        var c2 =new Vector3(calc(row1, new Vector3(0, 0, -1)), calc(row2, new Vector3(0, 0, -1)), calc(row3, new Vector3(0, 0, -1)));

        //print(Vector3.Distance(c1, c2) / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(MatrixStr != ""){//使用字符串矩阵
            
            StrToMat(MatrixStr);
        }

        Vector3 inputCoordinate = new Vector3(UDP.cam_x, UDP.cam_y, UDP.cam_z);
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
