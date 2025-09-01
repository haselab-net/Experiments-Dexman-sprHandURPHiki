using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//直接从LeapMotion到虚拟空间的校准
public class LeapDirectCalibrationProcess : MonoBehaviour
{
    public List<GameObject> calibrationCubes = new List<GameObject>();//所有要校准的方块扔进去
    public List<Vector4> from = new List<Vector4>();
    public List<Vector3> to = new List<Vector3>();
    int currentNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        calibrationCubes[currentNum].GetComponent<Renderer> ().material.color = new Color(255f/255f, 0, 0);//变成高亮色
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) {
            from.Add(new Vector4(LeapPinch.pinchPosition.x, LeapPinch.pinchPosition.y, LeapPinch.pinchPosition.z, 1));
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
    }
}
