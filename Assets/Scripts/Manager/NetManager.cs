using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.Collections.Concurrent;
using Newtonsoft.Json;

public class NetManager : IManager
{
    private static NetManager instance;
    private Thread thread;
    public TcpClient client;
    public NetworkStream stream;
    private LoginData response;
    private LoginData data;
    private int isLoadMatchScene = 0;
    private int isLoadGameScene = 0;
    private int LordNum;
    public int m_seatnum;


    public RoomInfo roomInfo;
    public PlayerInfo playerInfo;

    private ConcurrentQueue<string> messages = new ConcurrentQueue<string>();

    private MenuManager m_MenuManager;
    private UIManager m_UIManager;
    private CardManager m_CardManager;
    private HandManager m_HandManager;

    public CardsContainer LastCardContainer;

    public static NetManager Instance //单例模式
    {
        get
        {
            if (instance == null)
            {
                instance = new NetManager();
            }
            return instance;
        }
    }


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
        m_UIManager = managers[1] as UIManager;
        m_CardManager = managers[2] as CardManager;
        m_HandManager = managers[3] as HandManager;
    }

    // Start is called before the first frame update
    public void Start()
    {
        //初始化房间信息
        roomInfo = new RoomInfo();
        roomInfo.isCalled = false;//初始化没有叫地主

        playerInfo = new PlayerInfo();
    }
    public void Destroy()
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
            MenuManager.Instance.LoadMatchScene(data.isLogin);
        }

        if(isLoadGameScene == 1)
        {
            MenuManager.Instance.LoadGameScene();
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

        byte[] message = new byte[dataLength+4];
        Array.Copy(lengthBytes, 0, message, 0, dataLength);
        Array.Copy(netcodeBytes, 0, message, dataLength, 4);

/*        Array.Copy(lengthBytes, 0, message, 0, 4);
        Array.Copy(netcodeBytes, 0, message, 4, 4);*/

        stream.Write(message, 0, message.Length);
    }
    public void SendMessageToServer(int netcode, object messageObject)
    {
        string messageJson = JsonUtility.ToJson(messageObject);
        Debug.Log("Json Message to be sent: " + messageJson); // 输出要发送的Json信息

        byte[] netcodeBytes = BitConverter.GetBytes(netcode);
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageJson);

        int totalSize = netcodeBytes.Length + messageBytes.Length;
        Debug.Log("Total Size: " + totalSize); // 输出总的数据大小

        byte[] sizeBytes = BitConverter.GetBytes(totalSize);

        byte[] dataToSend = new byte[sizeBytes.Length + netcodeBytes.Length + messageBytes.Length];
        Buffer.BlockCopy(sizeBytes, 0, dataToSend, 0, sizeBytes.Length);
        Buffer.BlockCopy(netcodeBytes, 0, dataToSend, sizeBytes.Length, netcodeBytes.Length);
        Buffer.BlockCopy(messageBytes, 0, dataToSend, sizeBytes.Length + netcodeBytes.Length, messageBytes.Length);

        Debug.Log("Stream object: " + stream);

        try
        {
            stream.Write(dataToSend, 0, dataToSend.Length);
            Debug.Log("Data sent to server successfully!"); // 输出数据成功发送的信息
        }
        catch (Exception ex)
        {
            Debug.Log("Error when sending data to server: " + ex.Message); // 输出异常信息
        }

        Debug.Log("Data sent to server successfully!"); // 输出数据成功发送的信息
    }

    public void SendMessageToServer_net(int netcode, PlayerInfo playerInfo)
    {
        string messageJson = JsonConvert.SerializeObject(playerInfo); // 使用Json.NET来序列化
        Debug.Log("Json Message to be sent: " + messageJson); // 输出要发送的Json信息

        byte[] netcodeBytes = BitConverter.GetBytes(netcode);
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageJson);

        int totalSize = netcodeBytes.Length + messageBytes.Length;
        Debug.Log("Total Size: " + totalSize); // 输出总的数据大小

        byte[] sizeBytes = BitConverter.GetBytes(totalSize);

        byte[] dataToSend = new byte[sizeBytes.Length + netcodeBytes.Length + messageBytes.Length];
        Buffer.BlockCopy(sizeBytes, 0, dataToSend, 0, sizeBytes.Length);
        Buffer.BlockCopy(netcodeBytes, 0, dataToSend, sizeBytes.Length, netcodeBytes.Length);
        Buffer.BlockCopy(messageBytes, 0, dataToSend, sizeBytes.Length + netcodeBytes.Length, messageBytes.Length);

        Debug.Log("Stream object: " + stream);

        try
        {
            stream.Write(dataToSend, 0, dataToSend.Length);
            Debug.Log("Data sent to server successfully!"); // 输出数据成功发送的信息
        }
        catch (Exception ex)
        {
            Debug.Log("Error when sending data to server: " + ex.Message); // 输出异常信息
        }

        Debug.Log("Data sent to server successfully!"); // 输出数据成功发送的信息
    }


    void ReceiveMessage()
    {
        Debug.Log("ReceiveMessage");
        byte[] buffer = new byte[4096];
 

        while (true)
        {
            buffer = new byte[4096];
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

/*                Debug.Log("json_data=" + jsonData);
                data = JsonUtility.FromJson<LoginData>(jsonData);
                Debug.Log("isLogin=" + data.m_isRegisted);
                Debug.Log("isLogin=" + data.m_isLogin);*/
                
                switch (netcode)
                {
                    case NetCode.RSP_CREAT:
                        Debug.Log("json_data=" + jsonData);
                        data = JsonUtility.FromJson<LoginData>(jsonData);
                        Debug.Log("isLogin=" + data.m_isLogin);
                        isLoadMatchScene = 1;//修改变量值为1开始跳转场景
                        break;
                    case NetCode.RSP_ROOM_LIST:
                        Debug.Log("case NetCode.RSP_ROOM_LIST");
                        Debug.Log("Received room ID: " + jsonData[0]);
                        if (int.TryParse(jsonData[0].ToString(), out int roomId))
                        {
                            roomInfo.RoomID = roomId;
                        }
                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            MenuManager.Instance.LoadGameScene();
                        });
                        break;
                    case NetCode.RSP_SEAT_NUM:
                        Debug.Log("case NetCode.REQ_SEAT_NUM");
                        Debug.Log("SeatNum = " + jsonData[0]);
                        if (int.TryParse(jsonData[0].ToString(), out int seatnum))
                        {
                            playerInfo.SeatNum = seatnum;
                            m_seatnum = seatnum;
                        }
                        break;
                    case NetCode.RSP_DEAL_POKER:
                        Debug.Log("NetCode.RSP_DEAL_POKER");
                        jsonData = jsonData.Trim(); // 删除可能存在的多余空白字符
                        string jsonToParse = "{\"cards\":" + jsonData + "}";
                        Debug.Log("jsonToParse: " + jsonToParse); // 打印出完整的 JSON 字符串以便于调试
                        CardsContainer container = JsonConvert.DeserializeObject<CardsContainer>(jsonToParse);
                        Card[] cards = container.cards;
                        HandManager.Instance.allCards.AddRange(cards);
                        foreach (Card card in cards)
                        {
                            Debug.Log("Card Value: " + card.value + ", Card Suit: " + card.suit + ", ValueWeight: " + card.ValueWeight + ", SuitWeight: " + card.SuitWeight);
                            UnityMainThreadDispatcher.Instance.Enqueue(() => HandManager.Instance.AddCard(card));
                        }
                        break;
                    case NetCode.RSP_DEAL_LORD:
                        if (int.TryParse(jsonData[0].ToString(), out int lordnum))
                        {
                            LordNum = lordnum;
                        }
                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            UIManager.Instance.DealLordImage(playerInfo.SeatNum, lordnum);
                        });
                        break;
                    case NetCode.RSP_DEAL_LORD_CARD:
                        jsonData = jsonData.Trim(); // 删除可能存在的多余空白字符
                        string jsonToParse2 = "{\"cards\":" + jsonData + "}";
                        Debug.Log("LordCard: " + jsonToParse2); // 打印出完整的 JSON 字符串以便于调试
                        CardsContainer container_lord = JsonConvert.DeserializeObject<CardsContainer>(jsonToParse2);
                        Card[] cards_lord = container_lord.cards;
                        HandManager.Instance.allCards.AddRange(cards_lord);
                        HandManager.Instance.allCards.Sort((card1, card2) => card1.ValueWeight.CompareTo(card2.ValueWeight));
                        for (int i = 0; i < 3; i++)
                        {
                            int index = i; // 创建一个新的变量来保存 i 的当前值
                            UnityMainThreadDispatcher.Instance.Enqueue(() =>
                            {
                                UIManager.Instance.showLordCard(cards_lord[index].value, cards_lord[index].suit, index);//显示地主牌信息
                            });
                        }

                        if (playerInfo.SeatNum == LordNum)
                        {
                            UnityMainThreadDispatcher.Instance.Enqueue(() => HandManager.Instance.ClearAllCards());
                        }
                        Debug.Log("playerInfo.SeatNum="+ playerInfo.SeatNum);
                        Debug.Log("LordNum=" + LordNum);
                        foreach (Card card in HandManager.Instance.allCards)
                        {
                            Debug.Log("Lord Card Value: " + card.value + ", Card Suit: " + card.suit + ", ValueWeight: " + card.ValueWeight + ", SuitWeight: " + card.SuitWeight);
                            if (playerInfo.SeatNum == LordNum)
                            {
                                Debug.Log("Lord Card Value: " + card.value + ", Card Suit: " + card.suit + ", ValueWeight: " + card.ValueWeight + ", SuitWeight: " + card.SuitWeight);
                                UnityMainThreadDispatcher.Instance.Enqueue(() => HandManager.Instance.AddCard(card));
                            }
                        }
                        //地主显示出牌按钮
                        if (playerInfo.SeatNum == LordNum)
                        {
                            UnityMainThreadDispatcher.Instance.Enqueue(() => UIManager.Instance.showLordButton());
                        }
                        break;
                    case NetCode.RSP_PLAY_CARD:
                        string jsonToParse3 = jsonData.Trim();
                        Debug.Log("jsonToParse3: " + jsonToParse3); // 打印出完整的 JSON 字符串以便于调试
                        PlayerInfo playerInfo_recv = JsonConvert.DeserializeObject<PlayerInfo>(jsonToParse3);//接收到的上一个客户端的玩家信息
                                                                                                        // Make sure LastCardContainer is initialized
                        Debug.Log("PlayerInfo - SeatNum: " + playerInfo.SeatNum + ", CardNum: " + playerInfo.CardNum);
                        if (LastCardContainer == null)
                        {
                            LastCardContainer = new CardsContainer();
                        }

                        // Check if PlayCards is not null before trying to access it
                        if (playerInfo_recv.PlayCards != null)
                        {
                            LastCardContainer.cards = playerInfo_recv.PlayCards.ToArray();
                            Debug.Log("LastCardContainer.cards length: " + LastCardContainer.cards.Length);
                        }
                        else
                        {
                            Debug.LogError("PlayCards is null");
                        }
                        foreach (Card card in playerInfo_recv.PlayCards)
                        {
                            Debug.Log("playerInfo Card Value: " + card.value + ", Card Suit: " + card.suit + ", ValueWeight: " + card.ValueWeight + ", SuitWeight: " + card.SuitWeight);
                        }
                        foreach (Card card in LastCardContainer.cards)
                        {
                            Debug.Log("Last Player Card Value: " + card.value + ", Card Suit: " + card.suit + ", ValueWeight: " + card.ValueWeight + ", SuitWeight: " + card.SuitWeight);
                        }

                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            UIManager.Instance.showLordButton(); //收到上一个客户端的卡牌信息后，把当前客户端的出牌按钮显示
                            UIManager.Instance.isFirstPlayCard = false; //接收到了上一个客户端的信息，说明已经不是第一个出牌的了
                        });
                        break;
                    case NetCode.RSP_UNPLAY_CARD:
                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            UIManager.Instance.HideUnPlayCardButton();
                        });
                        break;
                }
            }
        }
    }
    
    public void syncUser() //发送数据前同步信息到Playerinfo中
    {
        playerInfo.SeatNum = m_seatnum;
        //playerInfo.CardNum =;
        playerInfo.PlayCards = HandManager.Instance.PlayCards;
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
