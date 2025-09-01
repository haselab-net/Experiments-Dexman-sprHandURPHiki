using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SprCs;
public class translationPlatForm : MonoBehaviour
{
    public float speed = 5f; // 移动速度
    private int direction = 1; // 1表示向右，-1表示向左

    void Update()
    {
        // 按1键时，改变方向为向右
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            direction = 1;
        }

        // 按2键时，改变方向为向左
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            direction = -1;
        }

        // 持续移动
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
    }
}
