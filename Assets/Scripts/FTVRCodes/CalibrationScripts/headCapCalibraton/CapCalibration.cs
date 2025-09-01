using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CapCalibration : MonoBehaviour
{
    int collectState = 0;
    Vector3 EyeCenterPosition, Eye1Position, Eye2Position, CapPosition;
    public GameObject TrackerCube, StereoCams, realHeadPosition;
    public float IPDPublic;
    public static float IPD = 0;
    public MatrixCalculation MC;
    LookAtConstraint LookAt;

    // Start is called before the first frame update
    void Start()
    {
        IPD = IPDPublic;
    }

    // Update is called once per frame
    void Update()
    {  
        var MyRotation = new Quaternion(-UDP.QuaternionX, -UDP.QuaternionY, -UDP.QuaternionZ, UDP.QuaternionW);//右手转左手
        
        var RotationEuler = MyRotation.eulerAngles;
        StereoCams.transform.rotation = Quaternion.Euler(RotationEuler.x, RotationEuler.y + 90, RotationEuler.z);//y轴差了90度不知道为什么 反正这样对了

        if(Input.GetKeyDown(KeyCode.Q)){
            if(collectState == 0)//eye1
            {
                Eye1Position = StereoCams.transform.position;
            }
            else if(collectState == 1)//eyeCenter
            {
                EyeCenterPosition = StereoCams.transform.position;//眼睛中心位置
            }
            else if(collectState == 2)//eye2
            {
                Eye2Position = StereoCams.transform.position;
                //EyeCenterPosition = (Eye1Position + Eye2Position) / 2f;
                //瞳距
                
                IPD = Vector3.Distance(Eye1Position, Eye2Position);
                print("IPD:" + IPD.ToString());
                //var result = new Vector3(calc(row1, inputCoordinate), calc(row2, inputCoordinate), calc(row3, inputCoordinate));

            }
            else if(collectState == 3){// head
                CapPosition = StereoCams.transform.position;
                TrackerCube.transform.position = CapPosition;
                realHeadPosition.transform.position = EyeCenterPosition;
                print("3");
                
            }
            else if(collectState == 4){
                var tmpPosition = realHeadPosition.transform.position;
                var tmpRotation = realHeadPosition.transform.rotation;
                TrackerCube.transform.rotation = StereoCams.transform.rotation;
                realHeadPosition.transform.position = tmpPosition;
                //realHeadPosition.transform.localPosition = new Vector3(0, 0, 0);
                realHeadPosition.transform.rotation = tmpRotation;
                LookAt = realHeadPosition.GetComponent<LookAtConstraint>();
                LookAt.enabled = false;
                print("4");
            }
            collectState++;
        }
        
    }
}
