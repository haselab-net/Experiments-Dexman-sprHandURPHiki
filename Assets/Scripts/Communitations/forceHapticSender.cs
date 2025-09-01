using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SprCs;
using UnityEditor;
using UnityEngine.UI;
using System;
using UnityEngine.Playables;
using System.IO;
using System.Text;

// send force to one finger
public class forceHapticSender : MonoBehaviour
{
    public byte fingerNum = 0, handNum = 0; // 0 right hand, 1 left hand
    
    private socketCore core;
    [Range(0, 512)]
    private short forceNum;

    public static PHSceneControl.rbContactValue myContact;
    private PHSceneIf phScene;
    private PHSolidBehaviour mySolid;
    private float lastFrameForce = 0f, lastLinearSpeed = 0, lastsecondFrameForce = 0f;

    public DrawHapticsWave drawHapticsWave; // if you want to display this force, assign this
    private float integralLinearVelocity = 0;

    private int isHapticOn = 1; // 1 on, 0 off
    public Text hapticText;

    [Header("1dof设置")]
    [Tooltip("用于输出振动和压力的电机ID")]
    public byte VibAndPressureMotor = 1;
    
    [Header("振动参数")]
    [Tooltip("速度对振动的影响系数")]
    [Range(0f, 10f)]
    public float velocityToVibration = 1.0f;
    
    [Tooltip("力对振动的影响系数")]
    [Range(0f, 10f)]
    public float forceToVibration = 1.0f;
    
    [Header("压力参数")]
    [Tooltip("压力输出增益系数")]
    [Range(0f, 5f)]
    public float pressureGain = 1.0f;

    [Header("碰撞参数")]
    //[Tooltip("检测碰撞的力阈值")]
    //public float collisionThreshold = 0.5f;
    [Tooltip("碰撞振动强度增益（基于速度）")]
    public float collisionVelocityMultiplier = 200f;

    [HideInInspector]
    public float relativeLinV, relativeAngularV, isStickslip, forceForLog;

    // Unity collision info storage
    private ContactPoint[] contacts = new ContactPoint[10];
    private int contactCount = 0;

    private Vector3 contactNormal;
    private Vector3 F_world; // global force
    private Vector3 tangentialForce; // tangential component
    private byte currentMaterialCode = 0x00; // Default material

    [Header("Motor Configurations")]
    public List<MotorConfig> motorConfigs = new List<MotorConfig>() { new MotorConfig() };

    // Force ranges for each axis
     [Header("力归一化范围")]
    public float forceCapX = 200f;
    public float forceCapY = 12f;
    public float forceCapZ = 12f;

    // New lower force thresholds for normalization
    [Tooltip("X轴力归一化下限")]
    public float forceLowerCapX = 0.01f;
    [Tooltip("Y轴力归一化下限")]
    public float forceLowerCapY = 0.01f;
    [Tooltip("Z轴力归一化下限")]
    public float forceLowerCapZ = 0.01f;
    
    [Header("力数据记录设置")]
    [Tooltip("是否启用力数据记录")]
    public bool enableForceLogging = false;
    [Tooltip("记录文件的路径和文件名（相对于应用程序的位置）")]
    public string forceLogFilePath = "force_log.csv";
    [Tooltip("记录的时间间隔（秒）")]
    public float logInterval = 0.1f;
    
    private float lastLogTime = 0f;
    private StreamWriter logWriter;
    private bool isLoggingInitialized = false;

    // Constants for vibration dominance over pressure
    private const short VIBRATION_THRESHOLD_FOR_PRESSURE_REDUCTION = 70; // If vibration strength is above this, pressure is reduced.
    private const float PRESSURE_REDUCTION_FACTOR_DUE_TO_VIBRATION = 0.5f; // Factor to reduce pressure by (e.g., 0.5 means 50% reduction).

    // 添加静态构造函数，用于在Unity编辑器中注册停止播放事件
    #if UNITY_EDITOR
    static forceHapticSender()
    {
        // 注册编辑器播放模式改变事件
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // 当编辑器即将退出播放模式时
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // 获取场景中所有的forceHapticSender实例
            forceHapticSender[] senders = FindObjectsOfType<forceHapticSender>();
            foreach (forceHapticSender sender in senders)
            {
                // 调用清理方法
                if (sender != null && sender.enabled)
                {
                    sender.StopAllMotors();
                }
            }
        }
    }
    #endif

    void Start()
    {
        // 仅在列表为空时添加一个默认电机作为示例
        if (motorConfigs.Count == 0)
        {
            // 添加一个默认电机配置作为示例
            motorConfigs.Add(new MotorConfig { 
                motorLabel = "A", 
                motorId = 1, 
                kxPos = 1f,
                kxNeg = 1f,
                kyPos = 1f,
                kyNeg = 1f,
                kzPos = 1f,
                kzNeg = 1f,
                lowerLimit = 0f, 
                upperLimit = 1f, 
                gain = 1f 
            });
            
            Debug.Log("已创建默认电机配置。请在Inspector中根据您的设备添加/配置电机。");
            Debug.Log("每个电机的kxPos/kxNeg等六个系数决定了它如何响应不同方向的力。");
        }
        
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            socketCore script = obj.GetComponent<socketCore>();
            if (script != null && script.socketHandNum == handNum)
                core = script;

            PHSceneBehaviour phb = obj.GetComponent<PHSceneBehaviour>();
            if (phb != null)
                phScene = phb.phScene;
        }

        myContact = new PHSceneControl.rbContactValue();
        myContact.resetValues();
        mySolid = GetComponent<PHSolidBehaviour>();
        
        // Initialize all motors to zero
        foreach (var motor in motorConfigs)
        {
            sendForce(0, motor.motorId);
        }
        
        // 初始化力数据记录
        if (enableForceLogging)
        {
            StartForceLogging();
        }
    }
    
    void OnDestroy()
    {
        Debug.Log("Component destroyed - stopping all motors");
        StopAllMotors();
        StopForceLogging();
    }

    private void StartForceLogging()
    {
        try
        {
            // 确保父目录存在
            string directory = Path.GetDirectoryName(forceLogFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // 打开文件流进行写入，使用UTF8编码
            logWriter = new StreamWriter(forceLogFilePath, false, Encoding.UTF8);
            
            // 写入表头
            logWriter.WriteLine("Time,ForceX,ForceY,ForceZ,TotalForce");
            
            isLoggingInitialized = true;
            Debug.Log($"开始记录力数据到: {Path.GetFullPath(forceLogFilePath)}");
        }
        catch (Exception e)
        {
            Debug.LogError($"无法启动力数据记录: {e.Message}");
            enableForceLogging = false;
            isLoggingInitialized = false;
        }
    }
    
    private void StopForceLogging()
    {
        if (isLoggingInitialized && logWriter != null)
        {
            logWriter.Flush();
            logWriter.Close();
            logWriter = null;
            isLoggingInitialized = false;
            Debug.Log("力数据记录已停止");
        }
    }
    
    private void LogForceData(float forceX, float forceY, float forceZ)
    {
        if (!isLoggingInitialized || logWriter == null)
            return;
            
        try
        {
            float time = Time.time;
            float totalForce = Mathf.Sqrt(forceX * forceX + forceY * forceY + forceZ * forceZ);
            
            // 写入一行数据到CSV，使用不变文化格式化小数（确保使用英文格式的小数点）
            logWriter.WriteLine(
                $"{time.ToString("F3", System.Globalization.CultureInfo.InvariantCulture)}," +
                $"{forceX.ToString("F3", System.Globalization.CultureInfo.InvariantCulture)}," +
                $"{forceY.ToString("F3", System.Globalization.CultureInfo.InvariantCulture)}," +
                $"{forceZ.ToString("F3", System.Globalization.CultureInfo.InvariantCulture)}," +
                $"{totalForce.ToString("F3", System.Globalization.CultureInfo.InvariantCulture)}"
            );
            
            // 定期刷新缓冲区，确保数据写入文件
            if (Time.frameCount % 30 == 0)
            {
                logWriter.Flush();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"记录力数据时出错: {e.Message}");
            StopForceLogging();
            enableForceLogging = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isHapticOn = 1 - isHapticOn;
            string status = isHapticOn == 0 ? "off" : "on";
            Debug.Log($"Haptics is now {status}");
            if (hapticText != null)
                hapticText.text = $"Haptics is now {status}";
        }
        
        // 处理记录开关的变化
        if (enableForceLogging && !isLoggingInitialized)
        {
            StartForceLogging();
        }
        else if (!enableForceLogging && isLoggingInitialized)
        {
            StopForceLogging();
        }
    }

    bool lastisStaticFric = false;
    void FixedUpdate()
    {
        if (isHapticOn == 0)
        {
            sendTengentialForce(0, 2);
            sendRotationalFriction(0, 0, fingerNum);
            
            foreach (var motor in motorConfigs)
            {
                sendForce(0, motor.motorId);
            }
            sendLinearFriction(0, 0, VibAndPressureMotor);
            sendForce(0, VibAndPressureMotor);
            return;
        }

        GetSumForce(mySolid.phSolid, myContact);
        currentMaterialCode = ParseMaterialFromObjectName(myContact.contactName); // 解析材质
        forceForLog = myContact.sumForce;

        Vector3 position = transform.position;
        float scale = 1f;
        F_world = new Vector3((float)myContact.fsum.x * scale, (float)myContact.fsum.y * scale, (float)myContact.fsum.z * scale);
        float normalMagnitude = Vector3.Dot(F_world, contactNormal);
        Vector3 normalForce = contactNormal * normalMagnitude;
        tangentialForce = F_world - normalForce;
        Vector3 localForce = transform.InverseTransformDirection(tangentialForce);
        
        // Visualization (optional)
        // Debug.DrawRay(position, transform.right * localForce.x * 1.00f, Color.red);
        // Debug.DrawRay(position, transform.up * localForce.y * 1.00f, Color.green);
        // Debug.DrawRay(position, transform.forward * localForce.z * 1.00f, Color.blue);

        Vector3 outputForce = PHSceneControl.isLocalDirectionFeedabck ? localForce : tangentialForce;
        float tangentialForceMagnitude = outputForce.magnitude;
        float totalForceMagnitude = F_world.magnitude;
        //Debug.Log("Tangential Force Magnitude: " + tangentialForceMagnitude + ", Total Force Magnitude: " + totalForceMagnitude);
        
        // --- 碰撞检测 ---
        if (PHSceneControl.isCollisionOn && lastFrameForce == 0f && totalForceMagnitude > 0f)
        {
            // 碰撞振幅与碰撞时的相对速度成正比
            float collisionVelocity = myContact.relativeLinearVelocity;
            short amplitude = (short)Mathf.Clamp(collisionVelocity * collisionVelocityMultiplier, 0, 512);
            sendCollideHaptics(VibAndPressureMotor, amplitude);
            Debug.Log($"Collision detected! Velocity: {collisionVelocity}, Amplitude: {amplitude}");
        }
        
        float forceX = (float)outputForce.x;
        float forceY = (float)outputForce.y;
        float forceZ = (float)outputForce.z;

        short vibrationStrength = 0;
        if (PHSceneControl.isVibrationOn)
        {
            float velocityComponent = Mathf.Abs(myContact.relativeLinearVelocity) * velocityToVibration;
            float forceComponentForVib = totalForceMagnitude * forceToVibration;
            float totalVibration = velocityComponent * forceComponentForVib;
            vibrationStrength = (short)Mathf.Clamp(totalVibration, short.MinValue, short.MaxValue);
        }

        float calculatedPressureValue = 0f;

        // --- 压力计算 ---
        if (PHSceneControl.isPressureOn)
        {
            float forceLowerCap = Mathf.Sqrt(forceLowerCapX*forceLowerCapX + forceLowerCapY*forceLowerCapY + forceLowerCapZ*forceLowerCapZ);
            float forceCap = Mathf.Sqrt(forceCapX*forceCapX + forceCapY*forceCapY + forceCapZ*forceCapZ);
            
            if (totalForceMagnitude <= forceLowerCap)
            {
                calculatedPressureValue = 0f;
            }
            else
            {
                MotorConfig pressureMotorConfig = motorConfigs.Find(m => m.motorId == VibAndPressureMotor);
                float lowerLimit = 0f;
                float upperLimit = 512f; 
                if (pressureMotorConfig != null)
                {
                    lowerLimit = pressureMotorConfig.lowerLimit;
                    upperLimit = pressureMotorConfig.upperLimit;
                }
                float normalizedPressureForce = Mathf.InverseLerp(forceLowerCap, forceCap, totalForceMagnitude);
                calculatedPressureValue = normalizedPressureForce * (upperLimit - lowerLimit) + lowerLimit;
                calculatedPressureValue *= pressureGain;
            }

            if (PHSceneControl.isVibrationOn && vibrationStrength > VIBRATION_THRESHOLD_FOR_PRESSURE_REDUCTION)
            {
                calculatedPressureValue *= PRESSURE_REDUCTION_FACTOR_DUE_TO_VIBRATION;
            }
            
            sendForce((short)Mathf.Clamp(calculatedPressureValue, short.MinValue, short.MaxValue), VibAndPressureMotor);
        }
        
        // --- 切向力计算 ---
        if (PHSceneControl.isTengentOn && motorConfigs.Count > 0)
        {
            foreach (var motor in motorConfigs)
            {
                if (!motor.isEnabled) { sendForce(0, motor.motorId); continue; }
                
                // 如果压力模式开启，则跳过专用的压力/振动电机，避免覆盖压力值
                if (PHSceneControl.isPressureOn && motor.motorId == VibAndPressureMotor)
                {
                    continue;
                }
                
                float xContribution = 0, yContribution = 0, zContribution = 0;
                if (PHSceneControl.isTengentXOn && Mathf.Abs(forceX) > 0.1f && (forceX > 0 ? Mathf.Abs(motor.kxPos) > 0.01f : Mathf.Abs(motor.kxNeg) > 0.01f))
                {
                    float normX = Mathf.InverseLerp(forceLowerCapX, forceCapX, Mathf.Abs(forceX));
                    xContribution = (normX * (motor.upperLimit - motor.lowerLimit) + motor.lowerLimit) * (forceX > 0 ? motor.kxPos : motor.kxNeg);
                }
                if (PHSceneControl.isTengentYOn && Mathf.Abs(forceY) > 0.01f && (forceY > 0 ? Mathf.Abs(motor.kyPos) > 0.01f : Mathf.Abs(motor.kyNeg) > 0.01f))
                {
                    float normY = Mathf.InverseLerp(forceLowerCapY, forceCapY, Mathf.Abs(forceY));
                    yContribution = (normY * (motor.upperLimit - motor.lowerLimit) + motor.lowerLimit) * (forceY > 0 ? motor.kyPos : motor.kyNeg);
                }
                if (PHSceneControl.isTengentZOn && Mathf.Abs(forceZ) > 0.01f && (forceZ > 0 ? Mathf.Abs(motor.kzPos) > 0.01f : Mathf.Abs(motor.kzNeg) > 0.01f))
                {
                    float normZ = Mathf.InverseLerp(forceLowerCapZ, forceCapZ, Mathf.Abs(forceZ));
                    zContribution = (normZ * (motor.upperLimit - motor.lowerLimit) + motor.lowerLimit) * (forceZ > 0 ? motor.kzPos : motor.kzNeg);
                }
                float totalMotorForce = (xContribution + yContribution + zContribution) * motor.gain;
                sendForce((short)Mathf.Clamp(totalMotorForce, short.MinValue, short.MaxValue), motor.motorId);
            }
        }
        else if (!PHSceneControl.isPressureOn) // 如果两种力反馈模式都关闭，则将所有电机清零
        {
            // No force mode (neither pressure nor tangential)
            foreach (var motor in motorConfigs)
            {
                sendForce(0, motor.motorId);
            }
            sendForce(0, VibAndPressureMotor);
        }

        if (PHSceneControl.isVibrationOn)
        {
            sendLinearFriction(10, vibrationStrength, VibAndPressureMotor); 
        }
        else
        {
            sendLinearFriction(0, 0, VibAndPressureMotor); 
        }
        
        if (enableForceLogging && isLoggingInitialized && Time.time - lastLogTime >= logInterval)
        {
            LogForceData(Mathf.Abs(forceX), Mathf.Abs(forceY), Mathf.Abs(forceZ));
            lastLogTime = Time.time;
        }

        // 更新上一帧的力，为下一次碰撞检测做准备
        lastFrameForce = totalForceMagnitude;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered: {other.gameObject.name}");
        Vector3 point = other.ClosestPoint(transform.position);
    }

    void OnTriggerStay(Collider other)
    {
        // 获取最近点作为接触点
        Vector3 contactPoint = other.ClosestPoint(transform.position);
        
        // 计算方向向量（作为法向量）
        Vector3 direction = (contactPoint - transform.position).normalized;
        contactNormal = direction;
    }

    // 根据物体名称解析材质代号
    private byte ParseMaterialFromObjectName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName)) return 0x00;

        if (objectName.IndexOf("Wood", System.StringComparison.OrdinalIgnoreCase) >= 0) return 0x01;
        if (objectName.IndexOf("Plastic", System.StringComparison.OrdinalIgnoreCase) >= 0) return 0x02;
        if (objectName.IndexOf("Ceramic", System.StringComparison.OrdinalIgnoreCase) >= 0) return 0x03;
        if (objectName.IndexOf("Aluminum", System.StringComparison.OrdinalIgnoreCase) >= 0) return 0x04;
        if (objectName.IndexOf("Glass", System.StringComparison.OrdinalIgnoreCase) >= 0) return 0x05;
        if (objectName.IndexOf("Iron", System.StringComparison.OrdinalIgnoreCase) >= 0) return 0x06;
        if (objectName.IndexOf("Steel", System.StringComparison.OrdinalIgnoreCase) >= 0) return 0x07;

        return 0x00; // 默认材质
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log($"Trigger exited: {other.gameObject.name}");
    }

    void sendForce(short num, byte fingerNum)
    {
        List<byte> myData = new List<byte>();
        var (high, low) = ConvertShortToBytes(num);
        myData.Add(0x01); myData.Add(fingerNum); myData.Add(currentMaterialCode); myData.Add(high); myData.Add(low);
        myData.Add(0xFF); myData.Add(0xFE);
        lock (core.sendData) core.sendData.AddRange(myData);
    }

    void sendTengentialForce(short num, short direction)
    {
        List<byte> myData = new List<byte>();
        short absNum = Math.Abs(num);
        var (high, low) = ConvertShortToBytes(absNum);
        myData.Add(0x06); myData.Add(fingerNum); myData.Add(currentMaterialCode); myData.Add(high); myData.Add(low);
        myData.Add(direction == 1 ? (byte)0x01 : direction == -1 ? (byte)0x00 : (byte)0x02);
        myData.Add(0xFF); myData.Add(0xFE);
        lock (core.sendData) core.sendData.AddRange(myData);
    }

    void sendLinearFriction(short num, short currentForce, byte fingerNum)
    {
        List<byte> myData = new List<byte>();
        float forceScale = Mathf.InverseLerp(0, 150, Mathf.Abs(currentForce));
        short product = (short)(num * forceScale);
        var (high, low) = ConvertShortToBytes(product);
        var (highv, lowv) = ConvertShortToBytes(num);
        myData.Add(0x03); myData.Add(fingerNum); myData.Add(currentMaterialCode);
        myData.Add(high); myData.Add(low);
        myData.Add(highv); myData.Add(lowv);
        myData.Add(0xFF); myData.Add(0xFE);
        lock (core.sendData) core.sendData.AddRange(myData);
    }

    void sendRotationalFriction(short num, short currentForce, byte fingerNum)
    {
        List<byte> myData = new List<byte>();
        float forceScale = Mathf.InverseLerp(0, 150, Mathf.Abs(currentForce));
        short product = (short)(num * forceScale);
        var (high, low) = ConvertShortToBytes(product);
        myData.Add(0x04); myData.Add(fingerNum); myData.Add(currentMaterialCode);
        myData.Add(high); myData.Add(low);
        myData.Add(0xFF); myData.Add(0xFE);
        lock (core.sendData) core.sendData.AddRange(myData);
    }

    void sendCollideHaptics(byte num, short amplitude)
    {
        List<byte> myData = new List<byte>();
        var (high, low) = ConvertShortToBytes(amplitude);
        myData.Add(0x02); myData.Add(num); myData.Add(currentMaterialCode);
        myData.Add(high); myData.Add(low);
        myData.Add(0xFF); myData.Add(0xFE);
        lock (core.sendData) core.sendData.AddRange(myData);
    }

    public PHSceneControl.rbContactValue GetSumForce(PHSolidIf phsolid, PHSceneControl.rbContactValue myContact)
    {
        myContact.resetValues();
        Vec3d f = new Vec3d(), to = new Vec3d(), fsum = new Vec3d();
        Posed pose = new Posed();
        for (int i = 0; i < phScene.NContacts(); i++)
        {
            var contact = phScene.GetContact(i);
            contact.GetRelativeVelocity(f, to);
            if (contact.GetSocketSolid() == phsolid || contact.GetPlugSolid() == phsolid)
            {
                float relLin = (float)f.norm();
                myContact.relativeAngularVelocity = (float)(to.x + to.y + to.z);
                myContact.relativeLinearVelocity = relLin;
                myContact.isStaticFriction = contact.IsStaticFriction();
                contact.GetConstraintForce(f, to);
                if (contact.GetSocketSolid() == phsolid)
                {
                    myContact.contactName = contact.GetPlugSolid().GetName();
                    contact.GetPlugPose(pose);
                    fsum -= contact.GetPlugSolid().GetPose().Ori() * pose.Ori() * f;
                }
                else
                {
                    myContact.contactName = contact.GetSocketSolid().GetName();
                    contact.GetSocketPose(pose);
                    fsum += contact.GetSocketSolid().GetPose().Ori() * pose.Ori() * f;
                }
            }
        }
        myContact.contactRotation = new Quaternion((float)pose.Ori().x, (float)pose.Ori().y, (float)pose.Ori().z, (float)pose.Ori().w);
        myContact.sumForce = Mathf.Abs((float)fsum.x) + Mathf.Abs((float)fsum.z);
        myContact.fsum = fsum;
        myContact.pose = pose;
        return myContact;
    }

    // 添加一个公共方法用于停止所有电机
    public void StopAllMotors()
    {
        if (core == null) return;

        Debug.Log("Stopping all motors explicitly");
        
        try
        {
            // 停止所有电机配置列表中的电机
            foreach (var motor in motorConfigs)
            {
                if (motor != null)
                {
                    sendForce(0, motor.motorId);
                }
            }
            
            // 显式停止特效电机
            sendForce(0, VibAndPressureMotor);
            // 停止振动
            sendLinearFriction(0, 0, VibAndPressureMotor);
            // 停止其他效果
            sendTengentialForce(0, 2);
            sendRotationalFriction(0, 0, fingerNum);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error stopping motors: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application quitting - stopping all motors");
        StopAllMotors();
    }

    void OnDisable()
    {
        Debug.Log("Component disabled - stopping all motors");
        StopAllMotors();
    }

    // 添加应用暂停处理
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)  // 如果应用被暂停
        {
            Debug.Log("Application paused - stopping all motors");
            StopAllMotors();
        }
    }

    public (byte, byte) ConvertShortToBytes(short value)
    {
        byte highByte = (byte)(value >> 8);
        byte lowByte = (byte)value;
        return (highByte, lowByte);
    }
}

// Define a new serializable class for motor parameters
[System.Serializable]
public class MotorConfig
{
    [Header("基本配置")]
    [Tooltip("是否启用此电机")]
    public bool isEnabled = true;
    public string motorLabel = "A";
    public byte motorId = 1;  // Numeric motor ID to send to hardware
    
    [Header("X轴系数")]
    [Tooltip("X轴正向(右)力响应系数")]
    public float kxPos = 0f;
    [Tooltip("X轴负向(左)力响应系数")]
    public float kxNeg = 0f;
    
    [Header("Y轴系数")]
    [Tooltip("Y轴正向(远离)力响应系数")]
    public float kyPos = 0f;
    [Tooltip("Y轴负向(靠近)力响应系数")]
    public float kyNeg = 0f;
    
    [Header("Z轴系数")]
    [Tooltip("Z轴正向力响应系数")]
    public float kzPos = 0f;
    [Tooltip("Z轴负向(指腹压力)响应系数")]
    public float kzNeg = 0f;
    
    [Header("力输出范围")]
    public float lowerLimit = 0f;
    public float upperLimit = 1f;
    public float gain = 1f;
}






