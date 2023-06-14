using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Text;
using UnityEngine.SceneManagement;
using System;

public class MenuManager : IManager
{
    public string Account;
    public string Password;
    public string Password_again;

    public Button loginButton;
    private TcpClient client;

    private UIManager m_UIManager;
    private MenuManager m_MenuManager;
    private GameManager m_GameManager;
    private NetManager m_NetManager;

    private bool isSceneLoading = false;//确保只加载一次场景
    public void Init(params object[] managers)
    {
        m_UIManager = managers[0] as UIManager;
        m_NetManager = managers[1] as NetManager;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void userLogin()
    {
        Debug.Log("userLogin");

        LoginData m_LoginData = new LoginData();

        m_LoginData.dataAccount = m_UIManager.UsernameInputFiled.text;
        m_LoginData.dataPassword = m_UIManager.PasswordInputFiled.text;

        Debug.Log("dataAccount=" + m_LoginData.dataAccount);
        Debug.Log("dataPassword=" + m_LoginData.dataPassword);

        string login_json = JsonUtility.ToJson(m_LoginData);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(login_json);
        //int json_length = jsonBytes.Length;
        int data_length = jsonBytes.Length+4;


        /*        //创建一个值为1的变量
                int netCode = 1;
                string login_data = json_length+ ","+ netCode + "," + login_json;
                Debug.Log("JSON data to send: " + login_data);

                byte[] data = Encoding.ASCII.GetBytes(login_data);
                //NetworkStream stream = m_NetManager.client.GetStream();
                m_NetManager.stream.Write(data, 0, data.Length);*/
        // Convert json_length to bytes
        //byte[] jsonLengthBytes = BitConverter.GetBytes(json_length);
        byte[] jsonLengthBytes = BitConverter.GetBytes(data_length);

        //创建一个值为1的变量
        int netCode = 1;
        // Convert netCode to bytes
        byte[] netCodeBytes = BitConverter.GetBytes(netCode);

        // Combine the bytes
        byte[] data = new byte[jsonLengthBytes.Length + netCodeBytes.Length + jsonBytes.Length];
        Buffer.BlockCopy(jsonLengthBytes, 0, data, 0, jsonLengthBytes.Length);
        Buffer.BlockCopy(netCodeBytes, 0, data, jsonLengthBytes.Length, netCodeBytes.Length);
        Buffer.BlockCopy(jsonBytes, 0, data, jsonLengthBytes.Length + netCodeBytes.Length, jsonBytes.Length);

        Debug.Log("JSON data to send: " + login_json);
        m_NetManager.stream.Write(data, 0, data.Length);


        //接收响应数据并打印
        /*        byte[] res_data = new byte[1024];
                int bytesRead = stream.Read(res_data, 0, res_data.Length);
                string response = Encoding.ASCII.GetString(res_data, 0, bytesRead);
                LoadMenu(response);*/
        //Debug.Log("response=" + response);
    }
    // Update is called once per frame
    public void Update()
    {
        
    }

    public void Destroy()
    {

    }

    public void LoadMatchScene(int judge)
    {
        if (judge == 1 && !isSceneLoading)
        {
            isSceneLoading = true;
            Debug.Log("LoadMatchScene");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Match");
            asyncLoad.completed += (AsyncOperation op) => {
                Debug.Log("Match Scene loaded");
                m_UIManager.Init(GameManager.Instance.m_MenuManager);
                isSceneLoading = false;
            };
        }
        else
        {
            Debug.Log("judge=0 or scene is already loading");
        }
    }


    public void LoadMenu(string response)
    {
        if(response == "isRegisted")
        {
            Debug.Log("isRegisted");
        }
        else 
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
            Debug.Log("notRegisted");
            asyncLoad.completed += (AsyncOperation op) => {
                Debug.Log("Scene loaded");
                m_UIManager.Init(GameManager.Instance.m_MenuManager);
            };
        }
    }
}
