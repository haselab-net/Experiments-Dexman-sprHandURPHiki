using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneMoveWithVirtualHand : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = target.transform.position;
        this.transform.rotation = target.transform.rotation;
    }
}
