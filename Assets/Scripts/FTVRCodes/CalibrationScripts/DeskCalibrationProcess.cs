using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//桌面校准
public class DeskCalibrationProcess : MonoBehaviour
{
    public List<GameObject> calibrationCubes = new List<GameObject>();//所有要校准的方块按顺序扔进去
    public List<Vector4> from = new List<Vector4>();
    public List<Vector3> to = new List<Vector3>();
    public MatrixCalculation mat;
    int currentNum = 0;
    
    /////桌面长宽比校准
    Vector3 point1, point2, point3;
    int currentNum2 = 0;
    public GameObject Plane;
    // Start is called before the first frame update
    void Start()
    {
        calibrationCubes[currentNum].GetComponent<Renderer> ().material.color = new Color(255f/255f, 0, 0);//变成高亮色

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            from.Add(new Vector4(UDP.cam_x, UDP.cam_y, UDP.cam_z, 1));
            to.Add(calibrationCubes[currentNum].transform.position);//采样点加入套餐
            //print(calibrationCubes[currentNum].transform.position);
            calibrationCubes[currentNum].GetComponent<Renderer> ().material.color = new Color(1, 1, 1);//恢复颜色
            currentNum++;
           
            if(currentNum < calibrationCubes.Count){    
                 calibrationCubes[currentNum].GetComponent<Renderer> ().material.color = new Color(255f/255f, 0, 0);//下一个变成高亮 
            }
            else{
                print("OK");
                LinsolveLib.LinsolveMatlab(from, to);
            }            
        }

        if (Input.GetKeyDown(KeyCode.W)){//桌面长宽比三点校准 左下 左上 右上
            if(currentNum == 0)
                point1 = new Vector3(UDP.cam_x, UDP.cam_y, UDP.cam_z);
            else if(currentNum == 1)
                point2 = new Vector3(UDP.cam_x, UDP.cam_y, UDP.cam_z);
            else if(currentNum == 2){
                point3 = new Vector3(UDP.cam_x, UDP.cam_y, UDP.cam_z);
                var tmpx = Mathf.Abs(point3.x - point2.x);
                var tmpz = Mathf.Abs(point2.z - point1.z);
                Plane.transform.localScale = new Vector3(tmpx / tmpz, Plane.transform.localScale.y, Plane.transform.localScale.z);//有问题 先不用了
            }
                
            currentNum++;
        }
    }
}
