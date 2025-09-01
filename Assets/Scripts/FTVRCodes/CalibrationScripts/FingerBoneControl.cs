using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class FingerBoneControl : MonoBehaviour
{
    //给HandTransform用的 控制指骨位置
    // Start is called before the first frame update
    public GameObject Palm, objectBone, tracker, offsetObject;//本手的手掌， 要追踪目标的指骨  校准后的跟踪点
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 myPinchPosition;
        if(tracker.name.Contains("Left"))
            myPinchPosition = LeapPinch.leftPinchPositin;
        else
            myPinchPosition = LeapPinch.pinchPosition;
        this.transform.rotation = objectBone.transform.rotation;
        this.transform.position = objectBone.transform.position - myPinchPosition + tracker.transform.position + offsetObject.transform.position;
    }
}
