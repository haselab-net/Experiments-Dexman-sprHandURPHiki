using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordiatesTransform : MonoBehaviour
{
    // Start is called before the first frame update //给坐标转换测试-圆形solid指尖用的
    public GameObject pinchSphere;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = LeapPinch.pinchPosition;
        if(Input.GetKeyDown(KeyCode.Space)){
            print(pinchSphere.transform.localPosition.ToString("F3"));
        }
    }
}
