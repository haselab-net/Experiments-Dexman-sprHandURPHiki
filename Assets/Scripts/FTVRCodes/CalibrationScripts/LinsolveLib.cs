using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LinsolveLib : MonoBehaviour
{
    // Start is called before the first frame update
    //public List<Vector4> aaaaa;
    void Start()
    {
        
        //  List<Vector4> from = new List<Vector4>();
        // from.Add(new Vector4(-0.160357103f,0.517897248f,-0.0907031298f, 1));
        // from.Add(new Vector4(0.0446990356f,0.501410127f,-0.0771433413f, 1));
        // from.Add(new Vector4(0.206262633f,0.498757839f,-0.07356029f, 1));
        // from.Add(new Vector4(0.216629997f,0.50010699f,0.0275836047f, 1));
        // List<Vector3> to = new List<Vector3>();
        // to.Add(new Vector3(0.220000476f,15.5402317f,-1.6299988f));
        // to.Add(new Vector3(0.929999828f,15.0902214f,-1.82999861f));
        // to.Add(new Vector3(1.32999945f,14.9702187f,-1.82999861f));
        // to.Add(new Vector3(1.30999947f,14.8602161f,-1.48999906f));
        // aaaaa = Linsolve(from, to);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void LinsolveMatlab(List<Vector4> from, List<Vector3> to){//输入两种点阵输出矩阵结果
        //List<string> from, List<string> to
        var fromPath = "./PointsInputList.csv";
        var toPath = "./PointsOutputList.csv";//输出矩阵给Python的csv 文件名和路径都不能改
        List<string> fromString = new List<string>();
        List<string> toString = new List<string>();
        for(int i = 0; i < from.Count;i++){//把采集到的点都转换为字符串装进去
            var tmpStr = from[i].ToString("F8");
            tmpStr = tmpStr.Replace("(", "");
            tmpStr = tmpStr.Replace(")", "");//一个点转字符串 去掉两边的括号
            fromString.Add(tmpStr);

            var tmpStr2 = to[i].ToString("F8");
            tmpStr2 = tmpStr2.Replace("(", "");
            tmpStr2 = tmpStr2.Replace(")", "");//一个点转字符串 去掉两边的括号
            toString.Add(tmpStr2);
        }
        WriteCsv(fromString, fromPath);
        WriteCsv(toString, toPath);//写入CSV
        print("DataForMatlabOK");
        //调用exe开始搞

    }
    public static List<Vector4> Linsolve(List<Vector4> from, List<Vector3> to){//输入两种点阵输出矩阵结果
        //List<string> from, List<string> to
        var fromPath = "./PointsInputList.csv";
        var toPath = "./PointsOutputList.csv";//输出矩阵给Python的csv 文件名和路径都不能改
        List<string> fromString = new List<string>();
        List<string> toString = new List<string>();
        for(int i = 0; i < from.Count;i++){//把采集到的点都转换为字符串装进去
            var tmpStr = from[i].ToString("F8");
            tmpStr = tmpStr.Replace("(", "");
            tmpStr = tmpStr.Replace(")", "");//一个点转字符串 去掉两边的括号
            fromString.Add(tmpStr);

            var tmpStr2 = to[i].ToString("F8");
            tmpStr2 = tmpStr2.Replace("(", "");
            tmpStr2 = tmpStr2.Replace(")", "");//一个点转字符串 去掉两边的括号
            toString.Add(tmpStr2);
        }
        WriteCsv(fromString, fromPath);
        WriteCsv(toString, toPath);//写入CSV
        //调用exe开始搞
        System.Diagnostics.Process exe = new System.Diagnostics.Process();
        exe.StartInfo.FileName = @".\LinSolveForUnity.exe";//读取的文件名为PointsInputList.csv PointsOutputList.csv, 必须放到Unity根目录
        exe.StartInfo.CreateNoWindow = false;
        exe.StartInfo.UseShellExecute = false;
        exe.StartInfo.RedirectStandardOutput = true;
        exe.StartInfo.StandardOutputEncoding = System.Text.Encoding.Default;
        exe.Start();
        string output = exe.StandardOutput.ReadToEnd();
        print(output);
        exe.WaitForExit();
        var OutputMatrix = output.Split(',');
        List<Vector4>  resultMatrix = new List<Vector4>();
        List<float>  resultFloatList = new List<float>();//最终结果
        for(int i = 0;i < OutputMatrix.Length;i++){//把字符串转成float塞进去
            resultFloatList.Add(float.Parse(OutputMatrix[i]));
        }
        Vector4 row1 = new Vector4(resultFloatList[0], resultFloatList[1], resultFloatList[2], resultFloatList[3]);
        Vector4 row2 = new Vector4(resultFloatList[4], resultFloatList[5], resultFloatList[6], resultFloatList[7]);
        Vector4 row3 = new Vector4(resultFloatList[8], resultFloatList[9], resultFloatList[10], resultFloatList[11]);
        resultMatrix.Add(row1);
        resultMatrix.Add(row2);
        resultMatrix.Add(row3);
        return resultMatrix;
    }
    public static void WriteCsv(List<string> strs, string path)
    {
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        //UTF-8方式保存
        using (StreamWriter stream = new StreamWriter(path, false))
        {
            for (int i = 0; i < strs.Count; i++)
            {
                if (strs[i] != null)
                    stream.WriteLine(strs[i]);
            }
        }
    }
}
