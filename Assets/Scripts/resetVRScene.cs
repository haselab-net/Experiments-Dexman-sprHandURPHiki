using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class resetVRScene : MonoBehaviour
{
    // Start is called before the first frame update
    XRInputSubsystem xRInputSubsystem;
    void Start()
    {
        xRInputSubsystem = new XRInputSubsystem();
        StartCoroutine(ExecuteAfterTime(2f));
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // If the player press left ctrl (listed as an input in Unity in the inputs window)...

        {   
            //print("ddd");
            //bool success = xRInputSubsystem.TryRecenter();
            
        UnityEngine.XR.InputTracking.Recenter();
    
        // ... we call the ResetOrientation() function of the OVRDevice script referenced in our ovrdevice variable.

        }


    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // 在延迟后执行的代码
        UnityEngine.XR.InputTracking.Recenter();
    }

}
