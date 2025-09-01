using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyDownSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject spawnObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M)){
            var a = Instantiate(spawnObject);
            a.transform.position = this.transform.position;
            a.SetActive(true);
        }
    }
}
