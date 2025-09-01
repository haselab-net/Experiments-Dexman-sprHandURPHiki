using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SprCs;
//负责管理物体掉光了自动生成的

public class objectRespawn : MonoBehaviour
{
    //public List<Color> colorlist;

    //public float deadY;
    float deadY = -0.2f;
    Vector3 initPosition;
    PHSolidBehaviour mySolid;
    Quaterniond initRotation;
    // Start is called before the first frame update
    public bool autoRespawn = true; // 控制是否自动恢复原位
    public bool respawnOnKeyPress = true; // 控制是否按R键恢复原位
    void Start()
    {
        initRotation = new Quaterniond();
        initPosition = this.transform.position;
        initRotation.x = this.transform.rotation.x;
        initRotation.y = this.transform.rotation.y;
        initRotation.z = this.transform.rotation.z;
        initRotation.w = this.transform.rotation.w;
        mySolid = this.GetComponent<PHSolidBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (autoRespawn && this.transform.position.y < deadY)
        {
            mySolid.UpdatePosition(initPosition, initRotation);
        }
        
        if (respawnOnKeyPress && Input.GetKeyDown(KeyCode.R))
        {
            mySolid.UpdatePosition(initPosition, initRotation);
        }

        if(Input.GetKeyDown(KeyCode.D)){
            mySolid.suddenDown();
        }
        
    }
}