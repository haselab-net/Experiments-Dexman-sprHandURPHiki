using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SprCs;

//SmoothMovement for scale experiment
public class objectMove : MonoBehaviour
{
    public float speed = 1.0f, offset = 1;
    Vector3 initPosition;
    PHSolidBehaviour mySolid;

    // 每个数字键对应的方向向量（打乱顺序）
    private Vector3[] directions = new Vector3[]
    {
        new Vector3(1, 0, 0),   // 1: 右
        new Vector3(-1, 0, 1),  // 2: 左上
        new Vector3(-1, 0, 0),  // 3: 左
        new Vector3(1, 0, -1),  // 4: 右下
        new Vector3(0, 0, 1),   // 5: 上
        new Vector3(0, 0, -1),  // 6: 下
        new Vector3(1, 0, 1),   // 7: 右上
        new Vector3(-1, 0, -1)  // 8: 左下
    };

    private Vector3 currentDirection;

    void Start()
    {
        initPosition = this.transform.position;
        mySolid = this.GetComponent<PHSolidBehaviour>();
        currentDirection = directions[0]; // 默认方向为按键1的方向
    }

    void Update()
    {
        // 检测按键 1 到 8，改变方向
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentDirection = directions[0];
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentDirection = directions[1];
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentDirection = directions[2];
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentDirection = directions[3];
        if (Input.GetKeyDown(KeyCode.Alpha5)) currentDirection = directions[4];
        if (Input.GetKeyDown(KeyCode.Alpha6)) currentDirection = directions[5];
        if (Input.GetKeyDown(KeyCode.Alpha7)) currentDirection = directions[6];
        if (Input.GetKeyDown(KeyCode.Alpha8)) currentDirection = directions[7];

        // 物体持续按照当前方向移动
        
        mySolid.UpdatePositionOnly(currentDirection * 0.1f);
    }
}
