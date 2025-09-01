using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmWebCapsule : MonoBehaviour
{
    //����capsule
    // Start is called before the first frame update
    public GameObject point1, point2;
    public bool isJustControlScale = false;//打开它可以让springHead正常
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isJustControlScale == false)
        {
            this.transform.position = (point1.transform.position - point2.transform.position) / 2 + point2.transform.position;
            var pinchVector = point1.transform.position - point2.transform.position;
            var pinchQuaterion = Quaternion.Euler(LookRotation(pinchVector));
            var pinchEuler = new Vector3(pinchQuaterion.eulerAngles.x + 90, pinchQuaterion.eulerAngles.y, pinchQuaterion.eulerAngles.z);
            this.transform.rotation = Quaternion.Euler(pinchEuler);
        }
        this.transform.localScale = new Vector3(transform.localScale.x, Vector3.Distance(point1.transform.position, point2.transform.position) / 2, transform.localScale.z);
        
    }

    public Vector3 LookRotation(Vector3 fromDir)
    {
        Vector3 eulerAngles = new Vector3();

        //AngleX = arc cos(sqrt((x^2 + z^2)/(x^2+y^2+z^2)))
        eulerAngles.x = Mathf.Acos(Mathf.Sqrt((fromDir.x * fromDir.x + fromDir.z * fromDir.z) / (fromDir.x * fromDir.x + fromDir.y * fromDir.y + fromDir.z * fromDir.z))) * Mathf.Rad2Deg;
        if (fromDir.y > 0) eulerAngles.x = 360 - eulerAngles.x;

        //AngleY = arc tan(x/z)
        eulerAngles.y = Mathf.Atan2(fromDir.x, fromDir.z) * Mathf.Rad2Deg;
        if (eulerAngles.y < 0) eulerAngles.y += 180;
        if (fromDir.x < 0) eulerAngles.y += 180;
        //AngleZ = 0
        eulerAngles.z = 0;
        return eulerAngles;
    }
}
