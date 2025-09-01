using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAllSpring : MonoBehaviour
{
    // Start is called before the first frame update、
    public Vector3 bias;
    Vector3 initLocalPosition;
    void Start()
    {
        bias = new Vector3(5, 0, 0);
        initLocalPosition = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localPosition = initLocalPosition + bias;
    }
}