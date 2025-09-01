using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableHandMeshRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    private bool state = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            if(state == true){
                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

                foreach (GameObject go in allObjects) {
                    if (go.name == "Capsule") {
                        go.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }
            else{
                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

                foreach (GameObject go in allObjects) {
                    if (go.name == "Capsule") {
                        go.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }
             
             state = !state;
        }
    }
}
