using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//废弃
public class MoveSpringhead : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject sprScene, targetDeskCenter;
    bool isFinish = false, isOver = false;
    //场景里只放一个桌子 当然墙是必须有的
    void Start()
    {

        
         //Invoke("TriggerEvent", 1f);
    }
    
    void FixedUpdate()
    {   


        if(isFinish == false){
            try{
                // GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                // foreach (GameObject go in allObjects)
                // {
                //     var tmp = go.GetComponent<PHSolidBehaviour>();
                //     if(tmp != null){
                //         tmp.isStop = true;
                //         tmp.IsDynamical = false;
                //     }
                // }    

                sprScene.transform.position = targetDeskCenter.transform.position;
                sprScene.transform.rotation = targetDeskCenter.transform.rotation;
                
                isFinish = true;
            }
            catch{
                
            }
        }
        
    }

}