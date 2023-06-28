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
    private HandManager m_HandManager;

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
/*        m_AudioManager = new AudioManager();
        m_UIManager = new UIManager();
        m_MenuManager = new MenuManager();
        m_NetManager = new NetManager();
        m_CardManager = new CardManager();
        m_HandManager = new HandManager();*/

        UnityMainThreadDispatcher.Instance.printInstance();
    }


    // Start is called before the first frame update
    public void Start()
    {
        m_WorldTrans = GameObject.Find(GameObjectPathInSceneDefine.WORLD_PATH).transform;
        m_UITrans = GameObject.Find(GameObjectPathInSceneDefine.UI_PATH).transform;

        Init();

        NetManager.Instance.connectToServer("127.0.0.1", 8888);

        NetManager.Instance.Start();
    }

    public void Init(params object[] managers)
    {
        MenuManager.Instance.Init(m_UIManager,m_NetManager,m_HandManager);
        UIManager.Instance.Init(m_MenuManager,m_NetManager);
        NetManager.Instance.Init(m_MenuManager,m_UIManager,m_CardManager,m_HandManager);
        CardManager.Instance.Init(m_UIManager);
        HandManager.Instance.Init(m_NetManager);
    }

    // Update is called once per frame
    public void Update()
    {
        //m_MenuManager.Update();
        //m_UIManager.Update();
        NetManager.Instance.Update();
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
