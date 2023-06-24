using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : IManager
{
    private AudioManager m_AudioManager;
    private UIManager m_UIManager;
    public MenuManager m_MenuManager;
    private NetManager m_NetManager;
    private CardManager m_CardManager;

    private Transform m_WorldTrans; //World×ø±ê
    private Transform m_UITrans; //UI×ø±ê

    private static GameManager m_Instance;
    public static GameManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new GameManager();
            }

            return m_Instance;
        }
    }
    public void Awake()
    {
        m_AudioManager = new AudioManager();
        m_UIManager = new UIManager();
        m_MenuManager = new MenuManager();
        m_NetManager = new NetManager();
        m_CardManager = new CardManager();

        UnityMainThreadDispatcher.Instance.printInstance();
    }


    // Start is called before the first frame update
    public void Start()
    {
        m_WorldTrans = GameObject.Find(GameObjectPathInSceneDefine.WORLD_PATH).transform;
        m_UITrans = GameObject.Find(GameObjectPathInSceneDefine.UI_PATH).transform;

        Init(null);

        m_NetManager.connectToServer("127.0.0.1", 8888);

        m_NetManager.Start();
    }

    public void Init(params object[] managers)
    {
        m_MenuManager.Init(m_UIManager,m_NetManager);
        m_UIManager.Init(m_MenuManager,m_NetManager);
        m_NetManager.Init(m_MenuManager,m_UIManager,m_CardManager);
        m_CardManager.Init(m_UIManager);
    }

    // Update is called once per frame
    public void Update()
    {
        m_MenuManager.Update();
        m_UIManager.Update();
        m_NetManager.Update();
    }

    public void Destroy()
    {
        //´ý²¹³ä
    }
    
    public MenuManager get_m_MenuManager()
    {
        return m_MenuManager;
    }
}
