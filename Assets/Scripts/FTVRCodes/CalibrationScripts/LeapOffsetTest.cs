using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//使用LeapMotion的偏移量来校准的方法 先控制手指尖 然后手移动到任何位置 接着控制LeapMotion位置与真手贴合 记录指尖坐标LocalPosiion和LeapMotion坐标
public class LeapOffsetTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject fingerTipsSphere, LeapHandController;
    public float MoveStep = 0.001f;
    public List<Vector3> PinchLocals = new List<Vector3>();
    public List<Vector3> OffsetS = new List<Vector3>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fingerTipsSphere.transform.position = LeapPinch.pinchPosition;

        if(Input.GetKey(KeyCode.LeftArrow)){
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
        else if(Input.GetKeyDown(KeyCode.Space)){
            PinchLocals.Add(fingerTipsSphere.transform.localPosition);
            OffsetS.Add(LeapHandController.transform.position);
        }
    }
}
