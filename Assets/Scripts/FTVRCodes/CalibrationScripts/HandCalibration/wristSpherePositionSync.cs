using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//为了保证获取到的手腕的点的位置始终是LeapController的子物体获取的 此脚本也赋给它
public class wristSpherePositionSync : MonoBehaviour
{
    public GameObject followedObject;//一般是ghostHand
    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = followedObject.transform.position;
    }
}
