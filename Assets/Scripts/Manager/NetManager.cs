using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;

public class NetManager : MonoBehaviour
{
    public TcpClient client;
    public NetworkStream stream;
    private LoginData response;

    public void connectToServer(string serverIP, int port)
    {
        //���ӷ�����
        // Create a TcpClient.
        client = new TcpClient(serverIP, port);

        // Get a client stream for reading and writing.
        stream = client.GetStream();
        StartCoroutine(ReadResponse());
    }

    public void Init(params object[] managers)
    {
        //...
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
        //�������������Ϣ
        byte[] data = Encoding.ASCII.GetBytes(msg);
        NetworkStream stream = client.GetStream();
        stream.Write(data, 0, data.Length);
    }

    IEnumerator ReadResponse()
    {
        Debug.Log("readResponse");
        // ȷ�����ݿ���
        while (stream.DataAvailable)
        {
            yield return null;
        }

        // ��ȡ�����ֶ�
        byte[] lengthBytes = new byte[4];
        stream.Read(lengthBytes, 0, 4);
        int length = BitConverter.ToInt32(lengthBytes, 0);

        // ��ȡnetCode�ֶ�
        byte[] netcodeBytes = new byte[4];
        stream.Read(netcodeBytes, 0, 4);
        int netcode = BitConverter.ToInt32(netcodeBytes, 0);

        if (netcode != 2)
        {
            Debug.LogError("Unexpected netcode");
            yield break;
        }

        // ��ȡJSON����
        byte[] jsonBytes = new byte[length];
        stream.Read(jsonBytes, 0, length);
        string json = Encoding.UTF8.GetString(jsonBytes);

        switch (netcode)
        {
            case NetCode.RSP_CREAT:
                response = JsonUtility.FromJson<LoginData>(json);
                break;
            //..�����
        }
        // ��JSON����ΪLoginResponse����
        //LoginResponse response = JsonUtility.FromJson<LoginResponse>(json);
        Debug.Log("IsLogin: " + response.isLogin);
    }
}
