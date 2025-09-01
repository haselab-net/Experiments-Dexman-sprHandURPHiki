using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UARTTest : MonoBehaviour
{
    private Socket _clientSocket;
    private byte[] _recieveBuffer = new byte[8142];
    private IPEndPoint _serverEndPoint;
    public byte data;

    // 开始连接
    void Start()
    {
        _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1233);  // 注意修改为你的服务器地址和端口
        _clientSocket.BeginConnect(_serverEndPoint, new AsyncCallback(ConnectCallback), null);
        InvokeRepeating("SendMessage", 0, 0.3f); // 每秒发送一次数据
    }

    // 连接成功回调
    private void ConnectCallback(IAsyncResult AR)
    {
        try
        {
            _clientSocket.EndConnect(AR);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    // 发送消息
    void SendMessage()
    {
        //byte[] sendData = Encoding.ASCII.GetBytes("Hello from Unity");
        byte[] sendData = {0x01, 0x02, 0xFF,0xFE,0x02, 0x00, 0xFF,0xFE,0x01, 0x02, 0x03, 0xFF,0xFE};
        _clientSocket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
    }

    // 发送消息回调
    private void SendCallback(IAsyncResult AR)
    {
        try
        {
            _clientSocket.EndSend(AR);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}
