using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getAnchor : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject sprScene;
    OVRSceneManager oM;
    bool isFinish = false, isOver = false;
    //场景里只放一个桌子 当然墙是必须有的
    void Start()
    {
        oM = new OVRSceneManager();
        oM.LoadSceneModel();
        
         //Invoke("TriggerEvent", 1f);
    }
    
    void FixedUpdate()
    {   
        // if(isOver == false){
        //     if(isFinish == true){
        //         GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        //         foreach (GameObject go in allObjects)
        //         {
        //             var tmp = go.GetComponent<PHSolidBehaviour>();
        //             if(tmp != null){
        //                 if(tmp.name != "Floor"){
        //                     tmp.isStop = false;
        //                     tmp.IsDynamical = true;
        //                 }
                        
        //             }
        //         }   
        //         isOver = true;
        //     }
        // }
        

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

                sprScene.transform.position = FindObjectsOfType<OVRScenePlane>()[0].transform.position;
                sprScene.transform.rotation = FindObjectsOfType<OVRScenePlane>()[0].transform.rotation;
                
                isFinish = true;
            }
            catch{
                
            }
        }
        
    }

}