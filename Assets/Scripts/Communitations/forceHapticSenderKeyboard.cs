using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SprCs;
using UnityEditor;
using UnityEngine.UI;
using System;
using UnityEngine.Playables;
// send force to one finger
public class forceHapticSenderKeyboard : MonoBehaviour
{
    public byte fingerNum = 0, handNum = 0;//0right hand, 1left hand
    socketCore core;
    [Range(0, 512)]
    short forceNum;

    public static PHSceneControl.rbContactValue myContact;
    PHSceneIf phScene;
    PHSolidBehaviour mySolid;
    private float lastFrameForce = 0f, lastLinearSpeed = 0, lastsecondFrameForce = 0f;

    public DrawHapticsWave drawHapticsWave; //if you want to display this force, assign this
    float integralLinearVelocity = 0;

    private int isHapticOn = 1;// 1 for on, 0 for off to control all currents

    public Text hapticText;

    [HideInInspector]
    public float relativeLinV, relativeAngularV, isStickslip, forceForLog; //专门给log用的
    void Start()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach(GameObject obj in allObjects)
        {
            socketCore script = obj.GetComponent<socketCore>();
            if(script != null && script.socketHandNum == handNum)
            {
                print(script.socketHandNum);
                core = script;
            }

            PHSceneBehaviour phb = obj.GetComponent<PHSceneBehaviour>();
            if(phb != null)
            {
                phScene = phb.phScene;
            }
        }

        myContact = new PHSceneControl.rbContactValue();
        myContact.resetValues();
        mySolid = this.GetComponent<PHSolidBehaviour>();
    }
    // Update is called once per frame
    bool lastisStaticFric = false;
    void Update()
    {
        
        short forceValue = 150;
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 左前
    {
        sendTengentialForce(forceValue, (short)(-1)); // 左 (-1 表示左)
        sendForce(forceValue, 6); // 前
        sendForce(0, 1); // 后置为0
    }
    else if (Input.GetKeyDown(KeyCode.Alpha2)) // 前
    {
        sendTengentialForce(0, 0); // 横向力为0
        sendForce(forceValue, 6); // 前
        sendForce(0, 1); // 后置为0
    }
    else if (Input.GetKeyDown(KeyCode.Alpha3)) // 右前
    {
        sendTengentialForce(forceValue, 1); // 右 (1 表示右)
        sendForce(forceValue, 6); // 前
        sendForce(0, 1); // 后置为0
    }
    else if (Input.GetKeyDown(KeyCode.Alpha4)) // 左
    {
        sendTengentialForce(forceValue, (short)(-1)); // 左 (-1 表示左)
        sendForce(0, 6); // 前置为0
        sendForce(0, 1); // 后置为0
    }
    else if (Input.GetKeyDown(KeyCode.Alpha5)) // 右
    {
        sendTengentialForce(forceValue, 1); // 右 (1 表示右)
        sendForce(0, 6); // 前置为0
        sendForce(0, 1); // 后置为0
    }
    else if (Input.GetKeyDown(KeyCode.Alpha6)) // 左后
    {
        sendTengentialForce(forceValue, (short)(-1)); // 左 (-1 表示左)
        sendForce(0, 6); // 前置为0
        sendForce(forceValue, 1); // 后
    }
    else if (Input.GetKeyDown(KeyCode.Alpha7)) // 后
    {
        sendTengentialForce(0, 0); // 横向力为0
        sendForce(0, 6); // 前置为0
        sendForce(forceValue, 1); // 后
    }
    else if (Input.GetKeyDown(KeyCode.Alpha8)) // 右后
    {
        sendTengentialForce(forceValue, 1); // 右 (1 表示右)
        sendForce(0, 6); // 前置为0
        sendForce(forceValue, 1); // 后
    }
    else if (Input.GetKeyDown(KeyCode.Space)) // 
    {
        sendTengentialForce(0, 1); // 右 (1 表示右)
        sendForce(0, 6); // 前置为0
        sendForce(0, 1); // 后
    }


        if(isHapticOn == 0)
        {
            sendTengentialForce(0, 2);
            sendLinearFriction(0, 0);
            sendRotationalFriction(0, 0);

            sendForce(0, 1);
            sendForce(0, 6);
            return;
        }
        // change forceNum here




        


        

    } 

    void sendForce(short num, byte fingerNum){
        
        List<byte> myData = new List<byte>();

        byte high, low;
        (high, low) = ConvertShortToBytes(num);

        myData.Add(0x01);
        myData.Add(fingerNum);
        myData.Add(high);
        myData.Add(low);
        myData.Add(0xFF);
        myData.Add(0xFE);
            
        lock (core.sendData)
        {
            core.sendData.AddRange(myData);
        }
        
    }





  void sendTengentialForce(short num, short direction){
    
    List<byte> myData = new List<byte>();

    short absNum = Math.Abs(num);

    byte high, low;
    (high, low) = ConvertShortToBytes(absNum); // 使用绝对值来拆分字节

    myData.Add(0x06);
    myData.Add(fingerNum);
    myData.Add(high);
    myData.Add(low);
    if(direction == 1)
        myData.Add(0x01); //sign
    else if(direction == -1)
        myData.Add(0x00);
    else
        myData.Add(0x02);

    myData.Add(0xFF);
    myData.Add(0xFE);

    lock (core.sendData)
    {
        core.sendData.AddRange(myData);
    }
}


    void sendLinearFriction(short num, short currentForce){ // velocity
        
        List<byte> myData = new List<byte>();

        byte high, low, highv, lowv;
        var force = 1 * Mathf.InverseLerp(0, 150, Mathf.Abs(currentForce));
        var product = (short)((float)num * force); //本来不应该乘的，这个是历史遗留
        (high, low) = ConvertShortToBytes(product);

        (highv, lowv) = ConvertShortToBytes(num);//加上一个速度传输

        myData.Add(0x03);
        myData.Add(fingerNum);
        myData.Add(high);
        myData.Add(low);
        myData.Add(highv);
        myData.Add(lowv);
        myData.Add(0xFF);
        myData.Add(0xFE);
            
        lock (core.sendData)
        {
            core.sendData.AddRange(myData);
        }
        
    }

    void sendRotationalFriction(short num, short currentForce){ // velocity
        
        List<byte> myData = new List<byte>();

        byte high, low, highv, lowv;
        var force = 1 * Mathf.InverseLerp(0, 150, Mathf.Abs(currentForce));
        var product = (short)((float)num * force);
        (high, low) = ConvertShortToBytes(product);

        (highv, lowv) = ConvertShortToBytes(num);//加上一个速度传输

        myData.Add(0x04);
        myData.Add(fingerNum);
        myData.Add(high);
        myData.Add(low);
        myData.Add(0xFF);
        myData.Add(0xFE);
            
        lock (core.sendData)
        {
            core.sendData.AddRange(myData);
        }
        
    }

    void sendCollideHaptics(byte num, short Amplitude){ //发送碰撞反馈信号
        
        List<byte> myData = new List<byte>();

        byte high, low;
        (high, low) = ConvertShortToBytes(Amplitude);

        myData.Add(0x02);
        myData.Add(num); //pulse motor num
        myData.Add(high);
        myData.Add(low); //pulse value 1
        myData.Add(0xFF); //pulse value 2
        myData.Add(0xFE); //STOP package
        lock (core.sendData)
        {
            core.sendData.AddRange(myData);
        }
        
    }



    bool DetectForce0ToNon0(float currentForce)
    {
        bool hasChanged = false;

        if (lastFrameForce < 0.0001 && currentForce > 0)
        {
            hasChanged = true;
            //print(currentForce);
        }

        lastFrameForce = currentForce;
        return hasChanged;
    }

    bool DetectForce0ToNon0Second(float currentForce)
    {
        bool hasChanged = false;

        if (lastsecondFrameForce < 0.0001 && currentForce > 0)
        {
            hasChanged = true;
            //print(currentForce);
        }

        lastsecondFrameForce = currentForce;
        return hasChanged;
    }

    bool DetectSpeedThreshold(float currentSpeed, float speedThreshold) {
        // 如果上一次速度小于阈值，但当前速度大于阈值，返回true
        if (lastLinearSpeed <= speedThreshold && currentSpeed > speedThreshold) {
            lastLinearSpeed = currentSpeed;
            return true;
        }
        lastLinearSpeed = currentSpeed;
        return false;
    }


    ////////dhal area
    private const double Sigma0 = 500; // 刚度
    private const double Fc = 8; // Coulomb摩擦力
    private const double ZMax = 0.01; // z的最大值，用于粘附映射
    private const double ZStick = 0.012;

    private double lastW = 0, lastX = 0; // 上一次的 w 值

    // 计算粘附映射函数
    private double Alpha(double zValue)
    {
        return (1 / ZMax) * Math.Pow(zValue, 8) / (Math.Pow(ZStick, 8) + Math.Pow(zValue, 8));
    }

    
    // 添加一个成员变量来存储上一帧的f值
    private double lastF = 0;

    public double ComputeStickSlip(double relativeX)
    {
        double z = relativeX - lastW;
        double y = Math.Abs(relativeX - lastX);
        lastX = relativeX;
        
        double deltaW;
        if ((Alpha(z) * Math.Abs(z)) > 1)
        {
            lastW = relativeX + (z > 0 ? ZMax : -ZMax);
        }
        else
        {
            deltaW = y * Alpha(z) * z;
            lastW += deltaW;
        }

        // 计算摩擦力f
        double f;
        if (Math.Abs(z) > ZMax)
        {
            f = Math.Sign(z) * Fc;
        }
        else
        {
            f = Sigma0 * z;
        }
        //求f的绝对值
        double absF = f;

        // 计算导数（当前帧f的绝对值与上一帧f的绝对值之差）
        double derivative = absF - lastF;
        lastF = absF; // 更新上一帧的f值

        if(Math.Abs(derivative) > 0.1) // 杂音 没有用
            return Math.Abs(derivative); // 返回摩擦力f的绝对值的导数
        else
            return 0;
    }

    ////////dhal area


    public PHSceneControl.rbContactValue GetSumForce(PHSolidIf phsolid, PHSceneControl.rbContactValue myContact){
        myContact.resetValues();
        Vec3d f = new Vec3d(0, 0, 0);
        Vec3d to = new Vec3d(0, 0, 0);
        Posed pose = new Posed();
        Vec3d fsum = new Vec3d(0, 0, 0);

        float relativeLinearSpeed = 0, secondObjectRelativeLinearSpeed = 0;
        for (int i = 0; i < phScene.NContacts(); i++){
                
                PHContactPointIf contact = phScene.GetContact(i);
                contact.GetRelativeVelocity(f, to);
                if(contact.GetSocketSolid() == phsolid || contact.GetPlugSolid() == phsolid){ //if one of the contact is the input solid

                    string secondObjectnameToFind = ""; //get secoond object haptics
                    if(contact.GetSocketSolid().GetName().Contains("Aim"))
                        secondObjectnameToFind = contact.GetSocketSolid().GetName().Substring(3);
                    else if(contact.GetPlugSolid().GetName().Contains("Aim"))
                        secondObjectnameToFind = contact.GetPlugSolid().GetName().Substring(3);
                    GameObject foundObject = GameObject.Find(secondObjectnameToFind);
                    if(foundObject != null){ //handle the second object 
                        secondObjectRelativeLinearSpeed = foundObject.GetComponent<AimObjectGetHaptic>().myContact.relativeLinearVelocity;
                        myContact.secondSumForce = foundObject.GetComponent<AimObjectGetHaptic>().myContact.sumForce;
                        //print(myContact.secondSumForce);
                    }
                        

                    //print(contact.GetSocketSolid().PHSolidBehaviour.gameObject.Name); // .GetComponent<objectGetForce>().myContact.sumForce
                    // contact.GetRelativeVelocity(f, to); //获取不到对应的gameObject, 作废
                    relativeLinearSpeed = (float)(f.x + f.y + f.z);
                    myContact.relativeAngularVelocity = (float)(to.x + to.y + to.z);
                    myContact.relativeLinearVelocity = relativeLinearSpeed;// + secondObjectRelativeLinearSpeed * 1; //第二个物体的速度也加上去
                    myContact.isStaticFriction = contact.IsStaticFriction();
                    
                    contact.GetConstraintForce(f, to);
                    if(contact.GetSocketSolid() == phsolid){
                        myContact.contactName = contact.GetPlugSolid().GetName();
                        contact.GetPlugPose(pose);
                        fsum -= contact.GetPlugSolid().GetPose().Ori() * pose.Ori() * f;
                        //tsum -= pose.Pos() % pose.Ori() * f;//扭矩
                    }
                    else if(contact.GetPlugSolid() == phsolid){
                        myContact.contactName = contact.GetSocketSolid().GetName();
                        contact.GetSocketPose(pose);
                        fsum += contact.GetSocketSolid().GetPose().Ori() * pose.Ori() * f;
                        //tsum += pose.Pos() % pose.Ori() * f;
                    }
                    

                }
                
                
        }
        myContact.contactRotation.x = (float)(pose.Ori().x);
        myContact.contactRotation.y = (float)(pose.Ori().y);
        myContact.contactRotation.z = (float)(pose.Ori().z);
        myContact.contactRotation.w = (float)(pose.Ori().w);
        
        myContact.sumForce = (Mathf.Abs((float)fsum.x)+ Mathf.Abs((float)fsum.z)); // + Mathf.Abs((float)fsum.y)   sqrt
        myContact.fsum = fsum;
        myContact.pose = pose;
        return myContact;
    }

    private void OnApplicationQuit()
    {
        // 在脚本被禁用时执行的操作，包括程序关闭 //没用
        Debug.Log("Script is being disabled. Perform cleanup here.");
        sendForce(0, fingerNum);
    }

    public (byte, byte) ConvertShortToBytes(short value)
    {
        byte highByte = (byte)(value >> 8);   // 获取高位字节
        byte lowByte = (byte)value;           // 获取低位字节

        return (highByte, lowByte);
    }
}

// public class rbContactValue{
//         public float relativeLinearVelocity = 0;
//         public float relativeAngularVelocity = 0;
//         public float sumForce = 0;
//         public string contactName = "";
//         public bool isStaticFriction = false;

//         public void resetValues(){
//             relativeLinearVelocity = 0;
//             relativeAngularVelocity = 0;
//             sumForce = 0;
//             contactName = "";
//             isStaticFriction = false;
//         }
// }



