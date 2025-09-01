using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update
    public Text timerText, FailText, palmRotationAVG;
    void Start()
    {
        
    }

    // Update is called once per frame
    public float timer = 0f, failNum = 0f;
    float pinchNormalDiffSum = 0, pinchNormalAvg = 0;
    float pinchNormalSumCount = 0;
    Vector3 LastPalmOrintation;
    // Update is called once per frame
    void Update()
    {
        //lastPalmDirection = 
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            timer = 0;
            pinchNormalDiffSum = 0;
            pinchNormalSumCount = 0;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            failNum++;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            failNum = 0;
        }
        pinchNormalDiffSum += Vector3.Angle(LeapPinch.PalmOrientation, LastPalmOrintation);
        //print(Vector3.Angle(LeapPinch.PalmOrientation, LastPalmOrintation));
        LastPalmOrintation = LeapPinch.PalmOrientation;
        pinchNormalSumCount++;
        pinchNormalAvg = pinchNormalDiffSum / pinchNormalSumCount;
        timer += Time.deltaTime;
        timerText.text = timer.ToString();
        FailText.text = failNum.ToString();
        palmRotationAVG.text = pinchNormalAvg.ToString();
        
    }
}
