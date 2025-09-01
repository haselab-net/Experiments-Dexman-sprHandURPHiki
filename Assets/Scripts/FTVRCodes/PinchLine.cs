using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchLine : MonoBehaviour
{
    public GameObject obj1, obj2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(obj1.transform.position, obj2.transform.position - obj1.transform.position);
    }
}
