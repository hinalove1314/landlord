using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Text;
using System;

[Serializable]
public class MyData
{
    [SerializeField]
    private int m_sceneIndex;//表示场景的编号，Menu为0
    [SerializeField]
    private string m_dataAccount;
    [SerializeField]
    private string m_dataPassword;

    public int sceneIndex
    {
        get{ return m_sceneIndex; }
        set { m_sceneIndex = value; }
    }

    public string dataAccount
    {
        get { return m_dataAccount; }
        set { m_dataAccount = value; }
    }
    public string dataPassword
    {
        get { return m_dataPassword; }
        set { m_dataPassword = value; }
    }
}

public class Login : MonoBehaviour
{
    public TMPro.TMP_InputField Account;
    public TMPro.TMP_InputField Password;
    private TcpClient client;


    public struct MyStruct
    {
        public int value1;
        public string value2;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            userLogin();
        }
    }

    public void userLogin()
    {
        client = new TcpClient();
        client.Connect("127.0.0.1", 8888);
        Debug.Log("Connected to server");

        MyData m_data = new MyData();
        m_data.sceneIndex = 0;
        m_data.dataAccount = Account.text;
        m_data.dataPassword = Password.text;
        Debug.Log("dataAccount=" + m_data.dataAccount);
        Debug.Log("dataPassword=" + m_data.dataPassword);


        string login_json = JsonUtility.ToJson(m_data);
        Debug.Log("JSON data to send: " + login_json);

        string username = Account.text;
        string password = Password.text;

        string message = "LOGIN|" + username + "|" + password;
       
        byte[] data = Encoding.ASCII.GetBytes(login_json);
        NetworkStream stream = client.GetStream();
        stream.Write(data, 0, data.Length);
        Debug.Log("Sent message: " + message);
    }
    public void isRegister()
    {
        //判断是否已经注册过
        
    }
}
