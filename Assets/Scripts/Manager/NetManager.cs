using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;

public class NetManager : MonoBehaviour
{
    public TcpClient client;
    public NetworkStream stream;

    public void connectToServer(string serverIP, int port)
    {
        //连接服务器
        // Create a TcpClient.
        client = new TcpClient(serverIP, port);

        // Get a client stream for reading and writing.
        stream = client.GetStream();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sendMsg(string msg)
    {
        //向服务器发送消息
        byte[] data = Encoding.ASCII.GetBytes(msg);
        NetworkStream stream = client.GetStream();
        stream.Write(data, 0, data.Length);
    }
}
