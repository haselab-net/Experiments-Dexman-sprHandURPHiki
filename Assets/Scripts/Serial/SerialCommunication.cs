using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;
using System.Text;
using System.Threading;
public class SerialCommunication : MonoBehaviour
{
    private SerialPort serialPort;
    public string port = "COM3";
    byte[] data;
Thread thread;
    void Start()
    {
        // 创建SerialPort对象
        serialPort = new SerialPort();

        // 设置串口名（如COM3）
        serialPort.PortName = port;

        // 设置波特率
        serialPort.BaudRate = 2000000;

        // 设置数据位
        serialPort.DataBits = 8;

        // 设置停止位
        serialPort.StopBits = StopBits.One;

        // 设置奇偶校验位
        serialPort.Parity = Parity.None;

        // 打开串口
        serialPort.Open();

        // 设置串口读取超时时间
        serialPort.ReadTimeout = 1000;

        // 设置串口写入超时时间
        serialPort.WriteTimeout = 1000;

        data = new byte[] { 0x31,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x21,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
        thread = new Thread(ThreadLoop);
    thread.Start();
    }

    private float time;
    public float frequency = 240f;
    private float amplitude = 1f;
    void Update()
    {
        
        // SendBytesOverSerial(data);
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     data[1] = 0x32;
        // }
        // if (Input.GetKeyDown(KeyCode.W))
        //     data[1] = 0;

        // time += Time.deltaTime;
        // float sineValue = Mathf.Sin(2f * Mathf.PI * frequency * time) * amplitude + 1;
        // sineValue *= 100;
        // data[1] = (byte)sineValue;
        // Debug.Log("Sine value: " + sineValue);
    }

    // void OnApplicationQuit()
    // {
    //     // 在应用程序退出时关闭串口
    //     if (serialPort != null && serialPort.IsOpen)
    //         serialPort.Close();
    // }

byte[] GenerateWaveform()
{
            time += 0.001f;
        float sineValue = Mathf.Sin(2f * Mathf.PI * frequency * time) * amplitude + 1;
        sineValue *= 100;
        data[1] = (byte)sineValue;
        Debug.Log("Sine value: " + sineValue);
    // 这里是你生成波形的代码
    return data;
}
    void ThreadLoop()
{
    while (true)
    {
        if (serialPort.IsOpen)
        {
            // 这里是你生成和发送波形的代码
            byte[] waveform = GenerateWaveform();
            serialPort.Write(waveform, 0, waveform.Length);
        }

        Thread.Sleep(1); // 暂停1毫秒
    }
}

void OnApplicationQuit()
{
    thread.Abort();
    if (serialPort.IsOpen)
    {
        serialPort.Close();
    }
}

    public void SendBytesOverSerial(byte[] bytes)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Write(bytes, 0, bytes.Length);
        }
        else
        {
            Debug.Log("Cannot send data as serial port is not open.");
        }
    }


    
}
