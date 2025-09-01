using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//批量控制一只手的各个组件

public class OneHandControl : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isVisualHandsOn = true, isPhysicsHandsOn = true;
    private bool privateIsVisualHandsOn = false, privateIsPhysicsHandsOn = false;
    public List<GameObject> VisualHands = new List<GameObject>();
    public List<GameObject> PhysicsHands = new List<GameObject>();
    void Start()
    {
        privateIsVisualHandsOn = !isVisualHandsOn;
        privateIsPhysicsHandsOn = !isPhysicsHandsOn;
    }

    // Update is called once per frame
    void Update()
    {
        if(privateIsVisualHandsOn != isVisualHandsOn){
            if(isVisualHandsOn == false){
                for(int i = 0;i < VisualHands.Count;i++){
                    VisualHands[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            else
            {
                for(int i = 0;i < VisualHands.Count;i++){
                    VisualHands[i].gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }

        if(privateIsPhysicsHandsOn != isPhysicsHandsOn){
            if(isPhysicsHandsOn == false){
                for(int i = 0;i < PhysicsHands.Count;i++){
                    PhysicsHands[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            else
            {
                for(int i = 0;i < PhysicsHands.Count;i++){
                    PhysicsHands[i].gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
        
        privateIsVisualHandsOn = isVisualHandsOn;
        privateIsPhysicsHandsOn = isPhysicsHandsOn;
    }
    
}


