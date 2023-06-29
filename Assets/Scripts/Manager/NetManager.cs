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

    public static NetManager Instance //����ģʽ
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
        //���ӷ�����
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
        //��ʼ��������Ϣ
        roomInfo = new RoomInfo();
        roomInfo.isCalled = false;//��ʼ��û�не���

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
        //�������������Ϣ
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
        Debug.Log("Json Message to be sent: " + messageJson); // ���Ҫ���͵�Json��Ϣ

        byte[] netcodeBytes = BitConverter.GetBytes(netcode);
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageJson);

        int totalSize = netcodeBytes.Length + messageBytes.Length;
        Debug.Log("Total Size: " + totalSize); // ����ܵ����ݴ�С

        byte[] sizeBytes = BitConverter.GetBytes(totalSize);

        byte[] dataToSend = new byte[sizeBytes.Length + netcodeBytes.Length + messageBytes.Length];
        Buffer.BlockCopy(sizeBytes, 0, dataToSend, 0, sizeBytes.Length);
        Buffer.BlockCopy(netcodeBytes, 0, dataToSend, sizeBytes.Length, netcodeBytes.Length);
        Buffer.BlockCopy(messageBytes, 0, dataToSend, sizeBytes.Length + netcodeBytes.Length, messageBytes.Length);

        Debug.Log("Stream object: " + stream);

        try
        {
            stream.Write(dataToSend, 0, dataToSend.Length);
            Debug.Log("Data sent to server successfully!"); // ������ݳɹ����͵���Ϣ
        }
        catch (Exception ex)
        {
            Debug.Log("Error when sending data to server: " + ex.Message); // ����쳣��Ϣ
        }

        Debug.Log("Data sent to server successfully!"); // ������ݳɹ����͵���Ϣ
    }

    public void SendMessageToServer_net(int netcode, PlayerInfo playerInfo)
    {
        string messageJson = JsonConvert.SerializeObject(playerInfo); // ʹ��Json.NET�����л�
        Debug.Log("Json Message to be sent: " + messageJson); // ���Ҫ���͵�Json��Ϣ

        byte[] netcodeBytes = BitConverter.GetBytes(netcode);
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageJson);

        int totalSize = netcodeBytes.Length + messageBytes.Length;
        Debug.Log("Total Size: " + totalSize); // ����ܵ����ݴ�С

        byte[] sizeBytes = BitConverter.GetBytes(totalSize);

        byte[] dataToSend = new byte[sizeBytes.Length + netcodeBytes.Length + messageBytes.Length];
        Buffer.BlockCopy(sizeBytes, 0, dataToSend, 0, sizeBytes.Length);
        Buffer.BlockCopy(netcodeBytes, 0, dataToSend, sizeBytes.Length, netcodeBytes.Length);
        Buffer.BlockCopy(messageBytes, 0, dataToSend, sizeBytes.Length + netcodeBytes.Length, messageBytes.Length);

        Debug.Log("Stream object: " + stream);

        try
        {
            stream.Write(dataToSend, 0, dataToSend.Length);
            Debug.Log("Data sent to server successfully!"); // ������ݳɹ����͵���Ϣ
        }
        catch (Exception ex)
        {
            Debug.Log("Error when sending data to server: " + ex.Message); // ����쳣��Ϣ
        }

        Debug.Log("Data sent to server successfully!"); // ������ݳɹ����͵���Ϣ
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
                        isLoadMatchScene = 1;//�޸ı���ֵΪ1��ʼ��ת����
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
                        jsonData = jsonData.Trim(); // ɾ�����ܴ��ڵĶ���հ��ַ�
                        string jsonToParse = "{\"cards\":" + jsonData + "}";
                        Debug.Log("jsonToParse: " + jsonToParse); // ��ӡ�������� JSON �ַ����Ա��ڵ���
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
                        jsonData = jsonData.Trim(); // ɾ�����ܴ��ڵĶ���հ��ַ�
                        string jsonToParse2 = "{\"cards\":" + jsonData + "}";
                        Debug.Log("LordCard: " + jsonToParse2); // ��ӡ�������� JSON �ַ����Ա��ڵ���
                        CardsContainer container_lord = JsonConvert.DeserializeObject<CardsContainer>(jsonToParse2);
                        Card[] cards_lord = container_lord.cards;
                        HandManager.Instance.allCards.AddRange(cards_lord);
                        HandManager.Instance.allCards.Sort((card1, card2) => card1.ValueWeight.CompareTo(card2.ValueWeight));
                        for (int i = 0; i < 3; i++)
                        {
                            int index = i; // ����һ���µı��������� i �ĵ�ǰֵ
                            UnityMainThreadDispatcher.Instance.Enqueue(() =>
                            {
                                UIManager.Instance.showLordCard(cards_lord[index].value, cards_lord[index].suit, index);//��ʾ��������Ϣ
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
                        //������ʾ���ư�ť
                        if (playerInfo.SeatNum == LordNum)
                        {
                            UnityMainThreadDispatcher.Instance.Enqueue(() => UIManager.Instance.showLordButton());
                        }
                        break;
                    case NetCode.RSP_PLAY_CARD:
                        string jsonToParse3 = jsonData.Trim();
                        Debug.Log("jsonToParse3: " + jsonToParse3); // ��ӡ�������� JSON �ַ����Ա��ڵ���
                        PlayerInfo playerInfo_recv = JsonConvert.DeserializeObject<PlayerInfo>(jsonToParse3);//���յ�����һ���ͻ��˵������Ϣ
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
                            UIManager.Instance.showLordButton(); //�յ���һ���ͻ��˵Ŀ�����Ϣ�󣬰ѵ�ǰ�ͻ��˵ĳ��ư�ť��ʾ
                            UIManager.Instance.isFirstPlayCard = false; //���յ�����һ���ͻ��˵���Ϣ��˵���Ѿ����ǵ�һ�����Ƶ���
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
    
    public void syncUser() //��������ǰͬ����Ϣ��Playerinfo��
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
            // ȷ�����ݿ���
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

            // ��ȡ�����ֶ�
            byte[] lengthBytes = new byte[4];
            stream.Read(lengthBytes, 0, 4);
            int length = BitConverter.ToInt32(lengthBytes, 0);

            // ��ȡnetCode�ֶ�
            byte[] netcodeBytes = new byte[4];
            stream.Read(netcodeBytes, 0, 4);
            int netcode = BitConverter.ToInt32(netcodeBytes, 0);

/*            if (netcode != 2)
            {
                Debug.LogError("Unexpected netcode");
                yield break;
            }*/

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
    //}
}
