using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginData
{
    [SerializeField]
    public int m_isRegisted;//1��ʾע��ɹ���0��ʾע��ʧ��
    [SerializeField]
    public int m_isLogin;//1��ʾ��¼�ɹ���0��ʾ��¼ʧ��
    [SerializeField]
    public string m_dataAccount;
    [SerializeField]
    public string m_dataPassword;

    public int isRegisted
    {
        get { return m_isRegisted; }
        set { m_isRegisted = value; }
    }
    public int isLogin
    {
        get { return m_isLogin; }
        set { m_isLogin = value; }
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
