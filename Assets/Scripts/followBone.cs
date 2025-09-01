using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//让一块SprBone Follow对应的Bone, 应该是只Follow位置和旋转
[ExecuteInEditMode]
public class followBone : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject aim, childSprSolid;
    public Vector3 rotationOffset, positionOffset, scaleOffset;

    public GameObject tracker, offsetObject;////本手的手掌，校准后的跟踪点 FTVR Only
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if (!Application.isPlaying && !UnityEditor.EditorApplication.isUpdating)
        {
            return; // 在编辑器模式下，只在必要时更新
        }

        if(aim != null){
            this.transform.position = aim.transform.position;
            var aimRotation = aim.transform.rotation;
            aimRotation = Quaternion.Euler(aimRotation.eulerAngles + rotationOffset);
            this.transform.rotation = aimRotation;
            if (Application.isPlaying)
            {
                // This code will only run when the game is in play mode
            }
            else
            {
                childSprSolid.transform.localPosition = positionOffset;
                childSprSolid.transform.localScale = scaleOffset;
            } 
        }  
        // if (aim != null) {
        //     Vector3 currentPosition = this.transform.position;
        //     Vector3 targetPosition = aim.transform.position;
        //     Quaternion currentRotation = this.transform.rotation;
        //     Quaternion targetRotation = aim.transform.rotation;
        //     targetRotation = Quaternion.Euler(targetRotation.eulerAngles + rotationOffset);

        //     // 计算位置变化量
        //     float positionChange = Vector3.Distance(currentPosition, targetPosition);

        //     // 如果位置变化量大于0.7米，则使用缓动插值
        //     if (positionChange > 0.5f) {
        //         float smoothingFactor = 0.03f; // 缓动系数，可以根据需要调整
        //         this.transform.position = Vector3.Lerp(currentPosition, targetPosition, smoothingFactor);
        //         this.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, smoothingFactor);
        //     } else {
        //         // 如果位置变化量小于等于0.7米，则直接设置位置和旋转
        //         this.transform.position = targetPosition;
        //         this.transform.rotation = targetRotation;
        //     }

        //     if (Application.isPlaying) {
        //         // This code will only run when the game is in play mode
        //     } else {
        //         childSprSolid.transform.localPosition = positionOffset;
        //         childSprSolid.transform.localScale = scaleOffset;
        //     } 
        // }
      
        if (Application.isPlaying)//PlayMode下才执行的
        {
            if(tracker != null){//如果跟 FTVR有关
                Vector3 myPinchPosition;
                if(tracker.name.Contains("Left"))
                    myPinchPosition = LeapPinch.leftPinchPositin;
                else
                    myPinchPosition = LeapPinch.pinchPosition;
                // this.transform.rotation = objectBone.transform.rotation;
                this.transform.position = this.transform.position - myPinchPosition + tracker.transform.position + offsetObject.transform.position;
            
            }
            
        }
        
        
    }

    // 添加这个方法
    void OnValidate()
    {
        // 在这里执行编辑器模式下需要的更新
        if (aim != null && childSprSolid != null)
        {
            transform.position = aim.transform.position;
            var aimRotation = aim.transform.rotation;
            aimRotation = Quaternion.Euler(aimRotation.eulerAngles + rotationOffset);
            transform.rotation = aimRotation;

            childSprSolid.transform.localPosition = positionOffset;
            childSprSolid.transform.localScale = scaleOffset;
        }
    }
}
