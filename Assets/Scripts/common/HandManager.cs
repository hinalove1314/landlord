using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HandManager : MonoBehaviour,IManager
{
    private static HandManager instance;
    public GameObject cardPrefab;  // ָ������Ԥ����
    public List<Card> PlayCards = new List<Card>();//����������
    public List<Card> allCards = new List<Card>();//����������
    public CardType cardType;
    public CardType LastCardType;

    //����һ����������lock����
    private readonly object lockObj = new object();

    private NetManager m_NetManager;

    public enum CardType
    {
        single_card,
        double_card,
        straight,
        double_straight,
        triple_straight,
        three,
        threeAndOne,
        threeAndTwo,
        fourAndOne,
        fourAndTwo,
        bomb,
        poker_bomb
    };
    public static HandManager Instance //����ģʽ
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("Instance of HandManager has not been set. Make sure HandManager is active and enabled in the scene.");
            }
            return instance;
        }
    }

    public void Init(params object[] managers)
    {
/*        Debug.Log("HandManage Init");
        m_NetManager = managers[0] as NetManager;
        if (m_NetManager == null)
        {
            Debug.Log("NetManager instance is null in HandManager.");
        }
        else
        {
            Debug.Log("NetManager instance is correctly initialized in HandManager.");
        }*/
    }

    public void SortCards(Card[] cards)
    {
        // ����һ�� Comparison<Card> ί�У���ʹ�� Card �� ValueWeight ���Խ��бȽ�
        Comparison<Card> comparison = (card1, card2) =>
        {
            // ��� card1 �� ValueWeight ���� card2 �� ValueWeight������ 1
            if (card1.ValueWeight > card2.ValueWeight)
            {
                return 1;
            }
            // ��� card1 �� ValueWeight С�� card2 �� ValueWeight������ -1
            else if (card1.ValueWeight < card2.ValueWeight)
            {
                return -1;
            }
            // ��� card1 �� ValueWeight ���� card2 �� ValueWeight������ 0
            else
            {
                return 0;
            }
        };

        // ʹ�� Array.Sort ������ comparison ί�ж� cards �����������
        Array.Sort(cards, comparison);
    }
    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("init hand instance");
            instance = this;
            DontDestroyOnLoad(gameObject);  // ȷ�������ڼ����³���ʱ���ٴ���Ϸ����
        }
        else if (instance != this)
        {
            Debug.Log("instance != this");
            Destroy(gameObject);
        }
        //m_NetManager = NetManager.Instance;
    }

    public void Start()
    {
        Debug.Log("HandManager start");
        Canvas canvasInScene = FindObjectOfType<Canvas>(); // Ѱ�ҵ�ǰ�����е�Canvas
        if (canvasInScene != null)
        {
            transform.SetParent(canvasInScene.transform, false); // ��CardPanel��Ϊ��Canvas��������
        }
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    public void Destroy()
    {
        
    }
    public void AddCard(Card cardData)
    {
        lock (lockObj)
        {
            Debug.Log("AddCard");

            // ʹ�� Instantiate ����ʵ����һ���µ��ƶ���
            GameObject cardObject = Instantiate(cardPrefab, transform);  // ʹ�´������ƶ����Ϊ cardPanel ���Ӷ���

            cardObject.SetActive(true);
            // �����ƶ�������ԣ���������Ƶ�ͼƬ��
            CardPrefab cardComponent = cardObject.GetComponent<CardPrefab>();
            cardComponent.SetCard(cardData);
        }
    }

    public void RemovePlayCards()
    {
        // ��ȡ���������е� CardPrefab ���
        CardPrefab[] allCardPrefabs = GameObject.FindObjectsOfType<CardPrefab>();

        foreach (Card card in PlayCards)
        {
            // �ҵ���Ӧ����Ϸ����
            CardPrefab cardPrefabToRemove = Array.Find(allCardPrefabs, cardPrefab => cardPrefab.m_cardData.Equals(card));

            if (cardPrefabToRemove != null)
            {
                // �� allCards �б����Ƴ�
                allCards.Remove(cardPrefabToRemove.m_cardData);

                // ������Ϸ����
                Destroy(cardPrefabToRemove.gameObject);
            }
        }

        // ��� PlayCards
        PlayCards.Clear();
    }

    public void PlayCardOnTable()
    {
        CardPrefab[] allCardPrefabs = GameObject.FindObjectsOfType<CardPrefab>();

        foreach (Card card in PlayCards)
        {
            CardPrefab cardPrefabToRemove = Array.Find(allCardPrefabs, cardPrefab => cardPrefab.m_cardData.Equals(card));
            cardPrefabToRemove.transform.parent = UIManager.Instance.m_Player1CardPanel;
        }
    }
    public void ClearAllCards()
    {
        lock (lockObj)
        {
            // ������Ƭ�б�����ÿһ�� GameObject
            foreach (Transform child in transform)
            {
                if (child.name == "Card")
                {
                    continue;
                }
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    public void CountCards()
    {
        PlayCards.Clear();  // ���HandCards�б���ֹ�ظ����
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Card(Clone)"))  // ����Ӷ�������ư��� "Card(Clone)"
            {
                CardPrefab cardPrefab = child.GetComponent<CardPrefab>();
                if (cardPrefab.isSelected)  // ��鿨���Ƿ�ѡ��
                {
                    PlayCards.Add(cardPrefab.m_cardData);
                }
            }
        }
    }

    public void PrintCards()//��ӡ����
    {
        foreach (Card card in PlayCards)
        {
            Debug.Log("Card Value: " + card.value + ", Card Suit: " + card.suit);
        }
    }

    public bool ConformRule() //�жϳ����Ƿ���Ϲ���
    {
        bool isConformRule = false;//�ж��Ƿ���ϳ��ƹ���
        int cardNum = PlayCards.Count;//��ȡ��Ҫ���Ƶĳ���
        Dictionary<string, int> cardCountDict = new Dictionary<string, int>();//����һ���ֵ���������ͬ�����Ŀ���
        bool hasFour = false;
        bool hasOne = false;
        bool hasThree = false;
        bool hasTwo = false;

        foreach (Card card in PlayCards)
        {
            if (cardCountDict.ContainsKey(card.value))
            {
                cardCountDict[card.value]++;
            }
            else
            {
                cardCountDict.Add(card.value, 1);
            }
        }

        //�������Ƴ�������������,�ȳ�����5�����ڵ����ͽ����ж�
        switch (cardNum)
        {
            case 1:
                cardType = CardType.single_card;
                isConformRule = true;
                break;
            case 2:
                if(PlayCards[0].ValueWeight == PlayCards[1].ValueWeight)
                {
                    cardType = CardType.double_card;
                    isConformRule =true;
                }
                if(PlayCards[0].ValueWeight>=14 && PlayCards[1].ValueWeight >= 15)//��ը
                {
                    cardType = CardType.poker_bomb;
                    isConformRule = true;
                }

                break;
            case 3:
                cardType = CardType.three;
                if(PlayCards[0].ValueWeight == PlayCards[1].ValueWeight && PlayCards[1].ValueWeight== PlayCards[2].ValueWeight)
                {
                    isConformRule = true;
                }
                break;
            case 4:
                if (PlayCards[0].ValueWeight == PlayCards[1].ValueWeight && 
                    PlayCards[1].ValueWeight == PlayCards[2].ValueWeight && 
                    PlayCards[2].ValueWeight == PlayCards[3].ValueWeight)//ը��
                {
                    cardType = CardType.bomb;
                    isConformRule = true;
                }
                else if ((PlayCards[0].ValueWeight == PlayCards[1].ValueWeight && 
                    PlayCards[1].ValueWeight == PlayCards[2].ValueWeight &&
                    PlayCards[2].ValueWeight != PlayCards[3].ValueWeight) || 
                    PlayCards[0].ValueWeight != PlayCards[1].ValueWeight &&
                    PlayCards[1].ValueWeight == PlayCards[2].ValueWeight &&
                    PlayCards[2].ValueWeight == PlayCards[3].ValueWeight)//����һ�����������ǰ��������ȣ������һ��,���һ�Ų���ȣ��������Ŵ���һ��
                {
                    cardType = CardType.threeAndOne;
                    isConformRule = true;
                }
                    break;
            case 5:
                foreach (KeyValuePair<string, int> entry in cardCountDict)
                {
                    if (entry.Value == 4)
                    {
                        hasFour = true;
                    }
                    else if (entry.Value == 1)
                    {
                        hasOne = true;
                    }
                    else if (entry.Value == 3)
                    {
                        hasThree = true;
                    }
                    else if (entry.Value == 2)
                    {
                        hasTwo = true;
                    }
                }

                if (hasFour && hasOne) //�Ĵ�һ
                {
                    cardType = CardType.fourAndOne;
                    isConformRule = true;
                }
                else if (hasThree && hasTwo) //������
                {
                    cardType = CardType.threeAndTwo;
                    isConformRule = true;
                }
                break;
            case 6:
                foreach (KeyValuePair<string, int> entry in cardCountDict)
                {
                    if (entry.Value == 4)
                    {
                        hasFour = true;
                    }
                    else if (entry.Value == 2)
                    {
                        hasTwo = true;
                    }

                    if(hasFour && hasTwo)//�Ĵ���
                    {
                        cardType = CardType.fourAndTwo;
                        isConformRule = true;
                    }
                }
                break;
        }

        if (cardNum >= 6)
        {
            bool isStraight = false;
            bool isDoubleStraight = false;
            bool isTripleStraight = false;
            foreach (KeyValuePair<string, int> entry in cardCountDict)
            {
                if (entry.Value == 1)
                {
                    hasOne = true;
                }
                else if (entry.Value == 2)
                {
                    hasTwo = true;
                }
                else if (entry.Value == 3)
                {
                    hasThree = true;
                }
                else if (entry.Value == 4)
                {
                    hasFour = true;
                }

                if(hasOne && hasTwo == false && hasThree == false && hasFour == false)//˳��
                {
                    isStraight = true;
                    for (int i = 0; i < cardNum-1; i++)
                    {
                        if((PlayCards[i+1].ValueWeight - PlayCards[i].ValueWeight != 1) || PlayCards[i].ValueWeight>12)//��һ���Ʊ�ǰһ���ƴ�1���Ҳ���Ϊ2���ߴ�С��
                        {
                            isStraight = false;
                            break;
                        }
                    }
                }
                else if (hasOne ==false && hasTwo == true && hasThree==false && hasFour==false)
                {
                    for (int i = 0; i < cardNum - 2; i++) 
                    { 
                        if((PlayCards[i+2].ValueWeight - PlayCards[i].ValueWeight !=1) || PlayCards[i].ValueWeight > 12)
                        {
                            isDoubleStraight = false;
                            break;
                        }
                    }
                }
                else if (hasOne == false && hasTwo == false && hasThree == true && hasFour == false)
                {
                    for (int i = 0; i < cardNum - 3; i++)
                    {
                        if ((PlayCards[i + 3].ValueWeight - PlayCards[i].ValueWeight != 1) || PlayCards[i].ValueWeight > 12)
                        {
                            isTripleStraight = false;
                            break;
                        }
                    }
                }
            }

            if (isStraight == true)
            {
                cardType = CardType.straight;
                isConformRule = true;
            }
            else if (isDoubleStraight == true)
            {
                cardType = CardType.double_straight;
                isConformRule = true;
            }
            else if(isTripleStraight == true)
            {
                cardType = CardType.triple_straight;
                isConformRule = true;
            }

        }

        return isConformRule;
    }

    public bool CompareCard() //true�ȽϽ���ɹ��������Ʊ���һ����Ҵ�false�ȽϽ��ʧ�ܣ������Ʊ���һ�����С
    {
        if(cardType != LastCardType)
        {
            return false;
        }

        Debug.Log("cardType=" + cardType);
        //�Բ�ͬ���͵������ж�
        if(cardType == CardType.single_card)
        {
            Debug.Log("playcard[0]=" + PlayCards[0].ValueWeight);
            Debug.Log("LastCardContainer.cards[0]" + NetManager.Instance.LastCardContainer.cards[0].ValueWeight);
            if (PlayCards[0].ValueWeight <= NetManager.Instance.LastCardContainer.cards[0].ValueWeight)
            {
                Debug.Log("PlayCards[0].ValueWeight <= NetManager.Instance.LastCardContainer.cards[0].ValueWeight");
                return false;
            }
        }
        else if (cardType == CardType.double_card)
        {
            Debug.Log("playcard[0]=" + PlayCards[0].ValueWeight);
            Debug.Log("LastCardContainer.cards[0]" + NetManager.Instance.LastCardContainer.cards[0].ValueWeight);
            if (PlayCards[0].ValueWeight <= NetManager.Instance.LastCardContainer.cards[0].ValueWeight)
            {
                return false;
            }
        }
        //else if(cardType == CardType.)

        return true;
    }

    public void sendPlayerInfoServer(PlayerInfo playerInfo) //���ͳ��Ƶ����ݵ������
    {
        NetManager.Instance.SendMessageToServer_net(41, playerInfo);
    }

    public void SendLastCardListToServer() //�����յ�������ֱ�ӷ��͸���һ���ͻ���
    {
        CardList cardList = new CardList();
        cardList.cards = new List<Card>(NetManager.Instance.LastCardContainer.cards);
        NetManager.Instance.SendMessageToServer(43, cardList);
    }
}
