using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorToCenter : MonoBehaviour
{
    public GameObject p1,cube, sprScene;
    public Vector3 positionOffset;
    bool isFinishScanAnchor = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        try{
            
            if(isFinishScanAnchor == false){//如果还没找着
                GetComponent<SpatialAnchorLoader>().LoadAnchorsByUuid();
                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

                foreach (GameObject go in allObjects) {
                    if (go.name.Contains("DemoAnchor")) {
                        p1 = go;
                        isFinishScanAnchor = true;
                        cube.transform.parent = go.transform;
                        cube.transform.rotation = p1.transform.rotation;
                        cube.transform.position = p1.transform.position;
                        cube.transform.localPosition += positionOffset;

                        sprScene.transform.position = cube.transform.position;
                        sprScene.transform.rotation = cube.transform.rotation;
                    }
                }
            }

            cube.transform.rotation = p1.transform.rotation;
            cube.transform.position = p1.transform.position;
            cube.transform.localPosition += positionOffset;
            

            
           
        }
        catch{

        }
    }
}
