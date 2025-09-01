using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandOffsetCalibrationProcess : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject fingerTipsSphere, LeapHandController;
    public float MoveStep = 0.001f;
    public List<Vector4> PinchLocals = new List<Vector4>();//from
    public List<Vector3> OffsetS = new List<Vector3>();//to
    public GameObject tagSphere;

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
            PinchLocals.Add(new Vector4(fingerTipsSphere.transform.localPosition.x, fingerTipsSphere.transform.localPosition.y, fingerTipsSphere.transform.localPosition.z, 1));
            OffsetS.Add(LeapHandController.transform.position);
            var tmpGO = Instantiate(tagSphere);
            tmpGO.transform.position = fingerTipsSphere.transform.position;
            
        }
        else if(Input.GetKeyDown(KeyCode.M)){
            LinsolveLib.LinsolveMatlab(PinchLocals, OffsetS);
            var meshr = fingerTipsSphere.GetComponent<MeshRenderer>();
            meshr.enabled = false;//让Pinch球消失方便测试
            print("LeapOffsetToMatlabOK");
        }

        
    }


    
}
