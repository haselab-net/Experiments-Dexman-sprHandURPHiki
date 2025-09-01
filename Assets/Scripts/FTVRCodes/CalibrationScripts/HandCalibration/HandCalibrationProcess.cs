using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//按照流程步骤的多点手校准
public class HandCalibrationProcess : MonoBehaviour
{
    public GameObject HandWristSphere, RightHandWristSphere, DisplaySphere, resultObjectTmp, resultObjectTmpRight, leftHandisOn;//追踪手腕的球 和用来做视觉效果的球
    public Vector3 handOffset;
    List<Vector4> from = new List<Vector4>();
    List<Vector3> to = new List<Vector3>();
    public Vector4 M1row1, M1row2, M1row3, M2row1, M2row2, M2row3;//LeapMotion手腕坐标转Tracker世界坐标的矩阵

    public MatrixCalculation MC;
    public string HandMatStr = "";
    // Start is called before the first frame update
    void Start()
    {//,,


        //,,



        M1row1 = new Vector4(1.098951729359698f,-0.061415811078968846f,-0.18446276407791723f,-0.12399791124128974f);//手
        M1row2 = new Vector4(0.03935770731908877f,-1.1886959158088826f,0.09106779161071012f,0.14900312043627184f);
        M1row3 = new Vector4(0.18519086647186303f,-0.0037553278739823615f,1.0132720537693631f,-1.779822430236909f);

        M2row1 = new Vector4(-23.122507345406714f,0.39774237718376027f,-4.696394801183594f,-12.18071942771724f);
        M2row2 = new Vector4(0.2095963897498313f,19.91756063888694f,-0.7335290649336592f,9.12595643327129f);
        M2row3 = new Vector4(-7.582452590958021f,0.3780691642998488f,25.464567386541734f,44.09622368044489f);
    }

    // Update is called once per frame
    void Update()
    {
        M2row1 = MC.row1;
        M2row2 = MC.row2;
        M2row3 = MC.row3;
        if(HandMatStr != ""){
            StrToMat(HandMatStr);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            if(leftHandisOn.activeSelf == true){
                from.Add(new Vector4(HandWristSphere.transform.localPosition.x, HandWristSphere.transform.localPosition.y, HandWristSphere.transform.localPosition.z, 1));//采样点加入套餐
                to.Add(new Vector3(UDP.cam_x, UDP.cam_y, UDP.cam_z));

                var tmp = Instantiate(DisplaySphere);//放个球表示这个点被采样过了
                var tmpVector3 = new Vector3(UDP.cam_x, UDP.cam_y, UDP.cam_z);
                tmp.transform.position = new Vector3(calc(M2row1, tmpVector3), calc(M2row2, tmpVector3), calc(M2row3, tmpVector3));
            }
            
        }
        else if(Input.GetKeyDown(KeyCode.M)){//为啥找不到Enter 最好是Enter
            LinsolveLib.LinsolveMatlab(from, to);
            print("HandCalibrationDataForMatlabOK");
        }

        //使用手校准矩阵的代码
        Vector3 inputCoordinate = new Vector3(HandWristSphere.transform.localPosition.x, HandWristSphere.transform.localPosition.y, HandWristSphere.transform.localPosition.z);
        //先转换为现实世界坐标
        var result = new Vector3(calc(M1row1, inputCoordinate), calc(M1row2, inputCoordinate), calc(M1row3, inputCoordinate));
        //然后再转换为虚拟世界坐标
        result = new Vector3(calc(M2row1, result), calc(M2row2, result), calc(M2row3, result));
        resultObjectTmp.transform.position = result + handOffset;
        //右手得再来一遍
        Vector3 inputCoordinateRight = new Vector3(RightHandWristSphere.transform.localPosition.x, RightHandWristSphere.transform.localPosition.y, RightHandWristSphere.transform.localPosition.z);
        //先转换为现实世界坐标
        var resultRight = new Vector3(calc(M1row1, inputCoordinateRight), calc(M1row2, inputCoordinateRight), calc(M1row3, inputCoordinateRight));
        //然后再转换为虚拟世界坐标
        resultRight = new Vector3(calc(M2row1, resultRight), calc(M2row2, resultRight), calc(M2row3, resultRight));
        resultObjectTmpRight.transform.position = resultRight + handOffset;
    }
    float calc(Vector4 row, Vector3 input){
        var tmp1 = row[0] * input[0];
        var tmp2 = row[1] * input[1];
        var tmp3 = row[2] * input[2];
        var tmp4 = row[3] * 1;
        return tmp1 + tmp2 + tmp3 + tmp4;
    }

    void StrToMat(string str){//输入字符串修改矩阵
        var OutputMatrix =  str.Split(',');
        List<Vector4>  resultMatrix = new List<Vector4>();
        List<float>  resultFloatList = new List<float>();//最终结果
        for(int i = 0;i < OutputMatrix.Length;i++){//把字符串转成float塞进去
            resultFloatList.Add(float.Parse(OutputMatrix[i]));
        }
        M1row1 = new Vector4(resultFloatList[0], resultFloatList[1], resultFloatList[2], resultFloatList[3]);
        M1row2 = new Vector4(resultFloatList[4], resultFloatList[5], resultFloatList[6], resultFloatList[7]);
        M1row3 = new Vector4(resultFloatList[8], resultFloatList[9], resultFloatList[10], resultFloatList[11]);
    }
}
