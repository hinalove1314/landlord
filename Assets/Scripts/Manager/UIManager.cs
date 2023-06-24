using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : IManager
{
    private InputField m_UsernameInputField;
    private InputField m_PasswordInputField;
    private InputField m_PasswordAgainInputField;
    private InputField m_AccountLoginInputField;
    private InputField m_PasswordLoginInputField;
    private Text m_PromptText;
    private Button m_StartGameButton;
    private Button m_RegisterButton;
    private Button m_RegisterPanelButton;
    private Button m_LoginButton;
    private Button m_PlayerReadyButton;
    private Button m_RegisterCloseButton;
    private Button m_StartGameCloseButton;
    private RectTransform m_RegisterPanel;
    private RectTransform m_StartGamePanel;


    //匹配场景
    private Button m_MatchButton;
    private RectTransform m_MatchPanel;
    private Button m_CancelButton;

    //游戏场景
    public GameObject m_cardPrefab;
    public GameObject m_cardPanel;
    private Button m_ToLandlordButton;
    private Button m_NotToLandlordButton;
    private Text m_LordText;
    private RectTransform m_PointPanel;
    private Image m_player1_image;
    private Image m_player2_image;
    private Image m_player3_image;

    private Image m_LordCard1_image;
    private Image m_LordCard2_image;
    private Image m_LordCard3_image;

    private MenuManager m_MenuManager;
    private NetManager m_NetManager;

    private bool StartGamepanelActive = false;
    private bool RegisterGamePanelActive = false;

    public void Init(params object[] managers)
    {
        Scene LoginScene = SceneManager.GetSceneByName("Login");
        if (LoginScene.IsValid() && LoginScene.buildIndex == 0)
        {
            foreach(GameObject rootObject in LoginScene.GetRootGameObjects())
            {
                Transform[] allChildren = rootObject.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if(child.gameObject.name == "AccountInputField")
                    {
                        m_UsernameInputField = child.gameObject.GetComponent<InputField>();
                    }
                    else if(child.gameObject.name == "PasswordInputField")
                    {
                        m_PasswordInputField = child.gameObject.GetComponent<InputField>();
                    }
                    else if(child.gameObject.name == "PasswordAgainInputField")
                    {
                        m_PasswordAgainInputField = child.gameObject.GetComponent<InputField>();
                    }
                    else if(child.gameObject.name == "AccountInputField2")
                    {
                        m_AccountLoginInputField = child.gameObject.GetComponent<InputField>();
                    }
                    else if (child.gameObject.name == "PasswordInputField2")
                    {
                        m_PasswordLoginInputField = child.gameObject.GetComponent<InputField>();
                    }
                    else if (child.gameObject.name == "StartGameButton")
                    {
                        m_StartGameButton = child.gameObject.GetComponent<Button>();
                    }
                    else if (child.gameObject.name == "RegisterButton")
                    {
                         m_RegisterButton = child.gameObject.GetComponent<Button>();
                    }
                    else if (child.gameObject.name == "LoginButton")
                    {
                        m_LoginButton = child.gameObject.GetComponent<Button>();
                    }
                    else if(child.gameObject.name == "RegisterPanelButton")
                    {
                        m_RegisterPanelButton = child.gameObject.GetComponent<Button>();
                    }
                    else if(child.gameObject.name == "StartGameCloseButton")
                    {
                        m_StartGameCloseButton = child.gameObject.GetComponent<Button>();
                    }
                    else if (child.gameObject.name == "RegisterCloseButton")
                    {
                        m_RegisterCloseButton = child.gameObject.GetComponent<Button>();
                    }
                    else if (child.gameObject.name == "StartGamePanel")
                    {
                        m_StartGamePanel = child.gameObject.GetComponent<RectTransform>();
                    }
                    else if (child.gameObject.name == "RegisterPanel")
                    {
                        m_RegisterPanel = child.gameObject.GetComponent<RectTransform>();
                    }
                    else if(child.gameObject.name == "PromptText")
                    {
                        m_PromptText = child.gameObject.GetComponent<Text>();
                    }
                }
            }
            m_MenuManager = managers[0] as MenuManager;
            m_NetManager = managers[1] as NetManager;
            m_StartGameButton.onClick.AddListener(OnStartGameButton); //打开开始游戏菜单
            m_RegisterButton.onClick.AddListener(OnRegisterButton);   //打开注册菜单
            m_StartGameCloseButton.onClick.AddListener(OnStartGameCloseButton);  //打开开始游戏后的关闭按钮
            m_RegisterCloseButton.onClick.AddListener(OnRegisterCloseButton);   //打开注册菜单后的关闭按钮
            m_LoginButton.onClick.AddListener(OnLoginButton);
            m_RegisterPanelButton.onClick.AddListener(OnRegisterPanelButton);
        }

        //匹配场景
        Scene MatchScene = SceneManager.GetSceneByName("Match");
        if (MatchScene.buildIndex == 1)
        {
            foreach (GameObject rootObject in MatchScene.GetRootGameObjects())
            {
                Transform[] allChildren = rootObject.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if (child.gameObject.name == "MatchButton")
                    {
                        m_MatchButton = child.gameObject.GetComponent<Button>();
                    }
                    else if(child.gameObject.name == "MatchPanel")
                    {
                        m_MatchPanel = child.gameObject.GetComponent<RectTransform>();
                    }
                    else if(child.gameObject.name == "CancelButton")
                    {
                        m_CancelButton= child.gameObject.GetComponent<Button>();
                    }
                }
            }
            m_MatchButton.onClick.AddListener(OnMatchButton);
            m_CancelButton.onClick.AddListener(OnCancelButton);
        }

        //战斗场景
        Scene GameScene = SceneManager.GetSceneByName("Game");
        if (GameScene.buildIndex == 2)
        {
            foreach (GameObject rootObject in GameScene.GetRootGameObjects())
            {
                Transform[] allChildren = rootObject.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if (child.gameObject.name == "Card")
                    {                      
                        m_cardPrefab = child.gameObject.GetComponent<GameObject>();
                    }
                    else if (child.gameObject.name == "CardPanel")
                    {
                        m_cardPanel = child.gameObject.GetComponent<GameObject>();
                    }
                    else if(child.gameObject.name == "ToLandlordButton")
                    {
                        m_ToLandlordButton = child.gameObject.GetComponent<Button>();
                    }
                    else if (child.gameObject.name == "NotToLandlordButton")
                    {
                        m_NotToLandlordButton = child.gameObject.GetComponent<Button>();
                    }
                    else if(child.gameObject.name == "LordText")
                    {
                        m_LordText = child.gameObject.GetComponent<Text>();
                    }
                    else if(child.gameObject.name == "PointPanel")
                    {
                        m_PointPanel = child.gameObject.GetComponent<RectTransform>();
                    }
                    else if(child.gameObject.name == "Player1_Image")
                    {
                        m_player1_image = child.gameObject.GetComponent<Image>();
                    }
                    else if (child.gameObject.name == "Player2_Image")
                    {
                        m_player2_image = child.gameObject.GetComponent<Image>();
                    }
                    else if (child.gameObject.name == "Player3_Image")
                    {
                        m_player3_image = child.gameObject.GetComponent<Image>();
                    }
                    else if (child.gameObject.name == "LordCard1")
                    {
                        m_LordCard1_image = child.gameObject.GetComponent<Image>();
                    }
                    else if (child.gameObject.name == "LordCard2")
                    {
                        m_LordCard2_image = child.gameObject.GetComponent<Image>();
                    }
                    else if (child.gameObject.name == "LordCard3")
                    {
                        m_LordCard3_image = child.gameObject.GetComponent<Image>();
                    }
                }
            }
            m_ToLandlordButton.onClick.AddListener(OnPointButton);
            m_NotToLandlordButton.onClick.AddListener(OnNoPointButton);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
    public void Destroy()
    {

    }

    public InputField UsernameInputFiled
    {
        get { return m_UsernameInputField; }
    }

    public InputField PasswordInputFiled
    {
        get { return m_PasswordInputField; }
    }
    public void GetUsernameInputFiled()
    {

    }
    public void GetPasswordInputField()
    {
        
    }

    public void OnPlayerReadyButton()
    {
        Debug.Log("PlayerReadyButton disabled");
        m_PlayerReadyButton.enabled = false;
    }
    public void OnStartGameButton()
    {
        m_StartGamePanel.gameObject.SetActive(true);
    }
    public void OnStartGameCloseButton()
    {
        m_StartGamePanel.gameObject.SetActive(false);
        m_PromptText.text = "";
    }
    public void OnRegisterButton()
    {
        m_RegisterPanel.gameObject.SetActive(true);
    }
    public void OnRegisterCloseButton()
    {
        m_RegisterPanel.gameObject.SetActive(false);
        m_PromptText.text = "";
    }

    //匹配场景

    public void OnMatchButton()
    {
        m_MatchPanel.gameObject.SetActive(true);
        m_NetManager.sendMsg(4, 13);
    }

    public void OnCancelButton()
    {
        m_MatchPanel.gameObject.SetActive(false);
    }

    //战斗场景
    public void OnPointButton()//叫地主
    {
        m_NetManager.roomInfo.isCalled = true;
        m_NetManager.SendMessageToServer(33,m_NetManager.roomInfo);
        m_PointPanel.gameObject.SetActive(false);
        m_LordText.text = "叫地主";
    }

    public void OnNoPointButton()//不叫地主
    {
        m_NetManager.roomInfo.isCalled = false;
        m_NetManager.SendMessageToServer(33, m_NetManager.roomInfo);
        m_PointPanel.gameObject.SetActive(false);
        m_LordText.text = "不叫";
    }

    public void DealLordImage(int player_seat,int lord_seat) //变更地主的头像
    {
        Debug.Log("DealLordImage");
        Debug.Log("player_seat=" + player_seat);
        Debug.Log("lord_seat=" + lord_seat);
        if (player_seat == lord_seat)
        {
            Debug.Log("DealLordImage1");
            Sprite newSprite = Resources.Load<Sprite>("Identity/Landlord"); // 更换为你地主图片的路径
            m_player1_image.sprite = newSprite;
        }
        else if(lord_seat-player_seat == 1 || lord_seat - player_seat==-2)
        {
            Debug.Log("DealLordImage2");
            Sprite newSprite = Resources.Load<Sprite>("Identity/Landlord"); // 更换为你地主图片的路径
            m_player2_image.sprite = newSprite;
        }
        else if(lord_seat - player_seat == 2 || lord_seat - player_seat==-1)
        {
            Debug.Log("DealLordImage3");
            Sprite newSprite = Resources.Load<Sprite>("Identity/Landlord"); // 更换为你地主图片的路径
            m_player3_image.sprite = newSprite;
        }
    }

    //显示地主牌
    public void showLordCard(string card_value, string card_suit,int card_num)
    {
        string imagePath;
        Debug.Log("showLordCard");
        Debug.Log("card_suit=" + card_suit);
        Debug.Log("card_value=" + card_value);
        imagePath = "Poker/" + card_suit + card_value;
        if (card_value == "big_joker")
        {
            imagePath = "Poker/" + "LJoker";
        }
        else if (card_value == "little_joker")
        {
            imagePath = "Poker/" + "SJoker";
        }
        // 使用 Resources.Load 函数来加载图片
        Sprite newSprite = Resources.Load<Sprite>(imagePath);

        // 更改 Image 组件的图片 
        if(card_num == 0)
        {
            Debug.Log("card_num == 1");
            m_LordCard1_image.sprite = newSprite;
        }else if(card_num == 1)
        {
            Debug.Log("card_num == 2");
            m_LordCard2_image.sprite = newSprite;
        }
        else if(card_num == 2)
        {
            Debug.Log("card_num == 3");
            m_LordCard3_image.sprite = newSprite;
        }
    }

    public void OnRegisterPanelButton()
    {
        if (string.IsNullOrEmpty(m_UsernameInputField.text))
        {
            m_PromptText.text = "用户名不能为空!";
            return;
        }
        if (string.IsNullOrEmpty(m_PasswordInputField.text))
        {
            m_PromptText.text = "密码不能为空!";
            return;
        }
        if(m_PasswordInputField.text != m_PasswordAgainInputField.text)
        {
            m_PromptText.text = "两次输入密码不一致!";
            return;
        }
        m_MenuManager.userLogin();
    }
    public void OnLoginButton()
    {
        if (string.IsNullOrEmpty(m_AccountLoginInputField.text))
        {
            m_PromptText.text = "用户名不能为空!";
            return;
        }
        if (string.IsNullOrEmpty(m_PasswordLoginInputField.text))
        {
            m_PromptText.text = "密码不能为空!";
            return;
        }
    }

}
