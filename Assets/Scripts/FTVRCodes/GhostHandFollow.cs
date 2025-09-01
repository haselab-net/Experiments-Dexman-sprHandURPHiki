using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostHandFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject follow;
    public Vector3 offset;
    Vector3 followPosition;
    void Start()
    {
        
    }
    void Update(){
        this.transform.position = offset + follow.transform.position;
    }
    // Update is called once per frame
}
