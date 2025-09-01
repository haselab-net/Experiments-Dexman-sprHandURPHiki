using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackingLostHint : MonoBehaviour
{
    //给leapMotion用的，丢手了UI提示
    public GameObject trackedObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!trackedObject.activeSelf){
            this.GetComponent<MeshRenderer>().enabled = true;
        }
        else
            this.GetComponent<MeshRenderer>().enabled = false;
    }
}
