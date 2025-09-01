using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;


public class LeapPinch : MonoBehaviour
{
    LeapProvider provider;
    // Start is called before the first frame update
    void Start()
    {
        provider = FindObjectOfType<LeapProvider>() as LeapProvider;
    }


    public static int hasRightHand = 0;
    public static float RightHandGrabAngle;
    public static Vector3 PalmOrientation, pinchPosition, leftPinchPositin;
    
    // Update is called once per frame
    void Update()
    {
        Frame frame = provider.CurrentFrame;
        if (frame.Hands.Count > 0)
        {
            foreach (Hand hand in frame.Hands)
            {
                if (hand.IsRight)
                {
                    hasRightHand = 1;
                }
            }

        }
        else if (frame.Hands.Count == 0)
        {
            hasRightHand = 0;
        }

        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsRight)
            {
                PalmOrientation = hand.GetPalmPose().rotation.eulerAngles;

                //RightHandGrabAngle = hand.GrabAngle; 
                pinchPosition = hand.GetPinchPosition();
                //print(RightHandGrabAngle);
/*                pinchDistance = hand.PinchDistance;//����0 �ɿ�100 ����30��������
                pinchTransform = hand.GetPinchPosition();
                //��дһ��������������
                //print(pinchTransform);

                pinchVector = (rightIndexPinch.transform.position - rightThumbPinch.transform.position).normalized;
                PalmVector = (rightThumbEndPinch.transform.position - rightIndexEndPinch.transform.position).normalized;
                //print("pinchDistance:" + pinchDistance);

                //palmRoration = hand.GetPalmPose().rotation;
                palmRoration = rightIndexPinch.transform.rotation;
                realPamlQuaternion = hand.GetPalmPose().rotation;
                pinchQuaterion = Quaternion.Euler(LookRotation(pinchVector));
                PalmQuaterion = Quaternion.Euler(LookRotation(PalmVector));

                realPalmRotation = hand.GetPalmPose().rotation.eulerAngles;

                palmNormal = hand.PalmNormal.ToVector3();*/

            }
            else if(hand.IsLeft){
                leftPinchPositin = hand.GetPinchPosition();
            }
        }
    }


    public Vector3 LookRotation(Vector3 fromDir)
    {
        Vector3 eulerAngles = new Vector3();

        //AngleX = arc cos(sqrt((x^2 + z^2)/(x^2+y^2+z^2)))
        eulerAngles.x = Mathf.Acos(Mathf.Sqrt((fromDir.x * fromDir.x + fromDir.z * fromDir.z) / (fromDir.x * fromDir.x + fromDir.y * fromDir.y + fromDir.z * fromDir.z))) * Mathf.Rad2Deg;
        if (fromDir.y > 0) eulerAngles.x = 360 - eulerAngles.x;

        //AngleY = arc tan(x/z)
        eulerAngles.y = Mathf.Atan2(fromDir.x, fromDir.z) * Mathf.Rad2Deg;
        if (eulerAngles.y < 0) eulerAngles.y += 180;
        if (fromDir.x < 0) eulerAngles.y += 180;
        //AngleZ = 0
        eulerAngles.z = 0;
        return eulerAngles;
        
    }
}
