using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//没用
public class getFrameRate : MonoBehaviour
{
    // Start is called before the first frame update
    TextMesh tm;
    void Start()
    {
        tm = this.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float fps = 1f / UnityEngine.Time.captureFramerate;;
        tm.text = "FrameRate: " + fps.ToString();
    }
}
