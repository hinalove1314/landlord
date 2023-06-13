using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.Collections.Concurrent;


public class NetManager : MonoBehaviour
{
    private Thread thread;
    public TcpClient client;
    public NetworkStream stream;
    private LoginData response;
    private LoginData data;
    private int isLoadMatchScene = 0;

    private ConcurrentQueue<string> messages = new ConcurrentQueue<string>();

    private MenuManager m_MenuManager;
    public void connectToServer(string serverIP, int port)
    {
        //连接服务器
        // Create a TcpClient.
        client = new TcpClient(serverIP, port);

        // Get a client stream for reading and writing.
        stream = client.GetStream();
        if (BitConverter.IsLittleEndian)
        {
            Debug.Log("System uses Little-Endian");
        }
        else
        {
            Debug.Log("System uses Big-Endian");
        }

        Debug.Log("Stream initialized: " + (stream != null));

        thread = new Thread(new ThreadStart(ReceiveMessage));
        thread.Start();
    }

    public void Init(params object[] managers)
    {
        m_MenuManager = managers[0] as MenuManager;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {
        string message;
        while (messages.TryDequeue(out message))
        {
            Debug.Log("Received Message: " + message);
        }

        if (isLoadMatchScene ==1)
        {
            m_MenuManager.LoadMatchScene(data.isLogin);
        }
    }

    public void OnDestroy()
    {
        thread.Abort();

        if (client != null)
        {
            client.Close();
        }
    }

/*    public void sendMsg(string msg)
    {
        //向服务器发送消息
        byte[] data = Encoding.ASCII.GetBytes(msg);
        NetworkStream stream = client.GetStream();
        stream.Write(data, 0, data.Length);
    }*/

    public void sendMsg(int dataLength,int netcode)
    {
        Debug.Log($"Start sendMsg!dataLength = {dataLength} netcode = {netcode}");
        byte[] lengthBytes = BitConverter.GetBytes(dataLength);
        byte[] netcodeBytes = BitConverter.GetBytes(netcode);

        byte[] message = new byte[dataLength];
        Array.Copy(lengthBytes, 0, message, 0, dataLength);
        Array.Copy(netcodeBytes, 0, message, dataLength, 4);

/*        Array.Copy(lengthBytes, 0, message, 0, 4);
        Array.Copy(netcodeBytes, 0, message, 4, 4);*/

        stream.Write(message, 0, message.Length);
    }

    void ReceiveMessage()
    {
        Debug.Log("ReceiveMessage");
        byte[] buffer = new byte[1024];
 

        while (true)
        {
            int length = stream.Read(buffer, 0, buffer.Length);
            if(length > 0)
            {
                byte[] dataBytes = new byte[4];
                Array.Copy(buffer, 0, dataBytes, 0, 4);
                int dataSize = BitConverter.ToInt32(dataBytes, 0);

                int netcode = BitConverter.ToInt32(buffer, 4);
                string jsonData = Encoding.UTF8.GetString(buffer, 8, dataSize);

                messages.Enqueue($"Data Size: {dataSize}, NetCode: {netcode}, JSON: {jsonData}");
                Debug.Log($"Data Size: {dataSize}, NetCode: {netcode}, JSON: {jsonData}");

                Debug.Log("json_data=" + jsonData);
                data = JsonUtility.FromJson<LoginData>(jsonData);
                Debug.Log("isLogin=" + data.m_isRegisted);
                Debug.Log("isLogin=" + data.m_isLogin);
                
                switch (netcode)
                {
                    case NetCode.RSP_CREAT:
                        isLoadMatchScene = 1;//修改变量值为1开始跳转场景
                        //Debug.Log("isLogin="+ response.isLogin);
                        break;
                }
            }
        }
    }


    public void ReadResponse()
    {
        Debug.Log("readResponse");
/*        while (true)
        {*/
            // 确保数据可用
/*            while (!stream.DataAvailable)
            {
                yield return new WaitForSeconds(0.1f);  // wait for 100 milliseconds
            }*/

            if (stream == null)
            {
                Debug.Log("stream null");
            }
            else
            {
                Debug.Log("stream not null");
            }

            // 读取长度字段
            byte[] lengthBytes = new byte[4];
            stream.Read(lengthBytes, 0, 4);
            int length = BitConverter.ToInt32(lengthBytes, 0);

            // 读取netCode字段
            byte[] netcodeBytes = new byte[4];
            stream.Read(netcodeBytes, 0, 4);
            int netcode = BitConverter.ToInt32(netcodeBytes, 0);

/*            if (netcode != 2)
            {
                Debug.LogError("Unexpected netcode");
                yield break;
            }*/

            // 读取JSON数据
            byte[] jsonBytes = new byte[length];
            stream.Read(jsonBytes, 0, length);
            string json = Encoding.UTF8.GetString(jsonBytes);

            switch (netcode)
            {
                case NetCode.RSP_CREAT:
                    response = JsonUtility.FromJson<LoginData>(json);
                    break;
                    //..待添加
            }
            // 将JSON解析为LoginResponse对象
            //LoginResponse response = JsonUtility.FromJson<LoginResponse>(json);
            Debug.Log("IsLogin: " + response.isLogin);
        }
    //}
}
