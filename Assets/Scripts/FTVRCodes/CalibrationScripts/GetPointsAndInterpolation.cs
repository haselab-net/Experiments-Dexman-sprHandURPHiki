using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPointsAndInterpolation : MonoBehaviour
{
    // Start is called before the first frame update
    public float MoveStep = 0.001f;
    public Vector3 LD0, RD0, LU0, LD1;
    public GameObject LD0Cube, RD0Cube, LU0Cube, LD1Cube;
    private int count = 4, currentCount = 0;
    public GameObject LeapHandController, originCube, pinchCube;
    private int calibratedFlag = 0;
    private Vector3 InitLeapHandPosition, InitoriginCubePosition;
    public bool OpenCalibration = false;
    void Start()
    {
        InitLeapHandPosition = LeapHandController.transform.position;
        InitoriginCubePosition = originCube.transform.position;
        if(OpenCalibration == false)
            calibratedFlag = 1;
    }

    // Update is called once per frame
    void Update()
    {
        pinchCube.transform.position = LeapPinch.pinchPosition;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch(currentCount)
            {
                case 0:
                    LD0 = LeapHandController.transform.position - InitLeapHandPosition;
                    break;
                case 1:
                    RD0 = LeapHandController.transform.position - InitLeapHandPosition;
                    break;
                case 2:
                    LU0 = LeapHandController.transform.position - InitLeapHandPosition;
                    break;
                case 3:
                    LD1 = LeapHandController.transform.position - InitLeapHandPosition;
                    break;
            }
                
            currentCount++;
            if(currentCount == 4){
                calibratedFlag = 1;
            }
        } 
        else if(Input.GetKey(KeyCode.LeftArrow)){
            LeapHandController.transform.position -= new Vector3(MoveStep, 0, 0);
        }
        else if(Input.GetKey(KeyCode.RightArrow)){
            LeapHandController.transform.position += new Vector3(MoveStep, 0, 0);
        }
        else if(Input.GetKey(KeyCode.UpArrow)){
            LeapHandController.transform.position += new Vector3(0, 0, MoveStep);
        }
        else if(Input.GetKey(KeyCode.DownArrow)){
            LeapHandController.transform.position -= new Vector3(0, 0, MoveStep);
        }
        else if(Input.GetKey(KeyCode.PageUp)){
            LeapHandController.transform.position += new Vector3(0, MoveStep, 0);
        }
        else if(Input.GetKey(KeyCode.PageDown)){
            LeapHandController.transform.position -= new Vector3(0, MoveStep,  0);
        }
        if(calibratedFlag == 1){
            var GlobalPinchPosition = LeapPinch.pinchPosition;
            //var LocalPinchPosition =  transform.parent.InverseTransformPoint(GlobalPinchPosition);
            var LocalPinchPosition = LeapPinch.pinchPosition;
            var LeapX = LD0.x * (1 - (LocalPinchPosition.x - originCube.transform.position.x) / (RD0Cube.transform.position.x - LD0Cube.transform.position.x)) + RD0.x * ((LocalPinchPosition.x - originCube.transform.position.x) / (RD0Cube.transform.position.x - LD0Cube.transform.position.x));
            var LeapY = LD0.y * (1 - (LocalPinchPosition.y - originCube.transform.position.y) / (LD1Cube.transform.position.y - LD0Cube.transform.position.y)) + LD1.y * ((LocalPinchPosition.y - originCube.transform.position.y) / (LD1Cube.transform.position.y - LD0Cube.transform.position.y));
            var LeapZ = LD0.z * (1 - (LocalPinchPosition.z - originCube.transform.position.z) / (LU0Cube.transform.position.z - LD0Cube.transform.position.z)) + LU0.z * ((LocalPinchPosition.z - originCube.transform.position.z) / (LU0Cube.transform.position.z - LD0Cube.transform.position.z));
            print("LocalPinchPosition.y" + LocalPinchPosition.y.ToString());
            print("originCube.transform.position.y" + originCube.transform.position.y.ToString());
            // print("RD0.x" + RD0.x.ToString());
            // print("RD0.y" + RD0.y.ToString());
            print(new Vector3(LeapX, LeapY, LeapZ));
            LeapHandController.transform.position = InitLeapHandPosition + new Vector3(LeapX, LeapY, LeapZ);
        }
        else{
            originCube.transform.position = InitoriginCubePosition;
        }
    }
}//8.42
