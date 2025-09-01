using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pulseHapticSender : MonoBehaviour
{
    socketCore core;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach(GameObject obj in allObjects)
        {
            socketCore script = obj.GetComponent<socketCore>();
            if(script != null)
            {
                core = script;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        // core.sendData.Add(0x02);
        // core.sendData.Add(0xAB);
        // core.sendData.Add(0xCD);
        // core.sendData.Add(0xFF);
        // core.sendData.Add(0xFE);
        if (Input.GetKeyDown(KeyCode.A))
        {
            print("aa");
            

            List<byte> myData = new List<byte>();
            myData.Add(0x02);
            myData.Add(0x05); //pulse motor num
            myData.Add(0x00);
            myData.Add(0xFD); //pulse value 1
            myData.Add(0xFF); //pulse value 2
            myData.Add(0xFE);
            lock (core.sendData)
            {
                core.sendData.AddRange(myData);
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            print("BB");
            List<byte> myData = new List<byte>();
            myData.Add(0x02);
            myData.Add(0x02);
            myData.Add(0x00);
            myData.Add(0xFD);
            myData.Add(0xFF);
            myData.Add(0xFE);
            lock (core.sendData)
            {
                core.sendData.AddRange(myData);
            }
        }

        
        

    }


    

    
}
