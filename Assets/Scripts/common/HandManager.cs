using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HandManager : MonoBehaviour,IManager
{
    private static HandManager instance;
    public GameObject cardPrefab;  // 指定卡牌预制体
    public List<Card> PlayCards = new List<Card>();//代表所有牌
    public List<Card> allCards = new List<Card>();//代表所有牌
    public CardType cardType;
    public CardType LastCardType;

    //定义一个对象用于lock操作
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
    public static HandManager Instance //单例模式
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
        // 创建一个 Comparison<Card> 委托，它使用 Card 的 ValueWeight 属性进行比较
        Comparison<Card> comparison = (card1, card2) =>
        {
            // 如果 card1 的 ValueWeight 大于 card2 的 ValueWeight，返回 1
            if (card1.ValueWeight > card2.ValueWeight)
            {
                return 1;
            }
            // 如果 card1 的 ValueWeight 小于 card2 的 ValueWeight，返回 -1
            else if (card1.ValueWeight < card2.ValueWeight)
            {
                return -1;
            }
            // 如果 card1 的 ValueWeight 等于 card2 的 ValueWeight，返回 0
            else
            {
                return 0;
            }
        };

        // 使用 Array.Sort 方法和 comparison 委托对 cards 数组进行排序
        Array.Sort(cards, comparison);
    }
    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("init hand instance");
            instance = this;
            DontDestroyOnLoad(gameObject);  // 确保不会在加载新场景时销毁此游戏对象
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
        Canvas canvasInScene = FindObjectOfType<Canvas>(); // 寻找当前场景中的Canvas
        if (canvasInScene != null)
        {
            transform.SetParent(canvasInScene.transform, false); // 将CardPanel设为新Canvas的子物体
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

            // 使用 Instantiate 函数实例化一个新的牌对象
            GameObject cardObject = Instantiate(cardPrefab, transform);  // 使新创建的牌对象成为 cardPanel 的子对象

            cardObject.SetActive(true);
            // 设置牌对象的属性，例如更改牌的图片等
            CardPrefab cardComponent = cardObject.GetComponent<CardPrefab>();
            cardComponent.SetCard(cardData);
        }
    }

    public void RemovePlayCards()
    {
        // 获取场景中所有的 CardPrefab 组件
        CardPrefab[] allCardPrefabs = GameObject.FindObjectsOfType<CardPrefab>();

        foreach (Card card in PlayCards)
        {
            // 找到对应的游戏对象
            CardPrefab cardPrefabToRemove = Array.Find(allCardPrefabs, cardPrefab => cardPrefab.m_cardData.Equals(card));

            if (cardPrefabToRemove != null)
            {
                // 从 allCards 列表中移除
                allCards.Remove(cardPrefabToRemove.m_cardData);

                // 销毁游戏对象
                Destroy(cardPrefabToRemove.gameObject);
            }
        }

        // 清空 PlayCards
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
            // 遍历卡片列表，销毁每一个 GameObject
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
        PlayCards.Clear();  // 清空HandCards列表，防止重复添加
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Card(Clone)"))  // 如果子对象的名称包含 "Card(Clone)"
            {
                CardPrefab cardPrefab = child.GetComponent<CardPrefab>();
                if (cardPrefab.isSelected)  // 检查卡牌是否被选中
                {
                    PlayCards.Add(cardPrefab.m_cardData);
                }
            }
        }
    }

    public void PrintCards()//打印卡牌
    {
        foreach (Card card in PlayCards)
        {
            Debug.Log("Card Value: " + card.value + ", Card Suit: " + card.suit);
        }
    }

    public bool ConformRule() //判断出牌是否符合规则
    {
        bool isConformRule = false;//判断是否符合出牌规则
        int cardNum = PlayCards.Count;//获取将要出牌的长度
        Dictionary<string, int> cardCountDict = new Dictionary<string, int>();//创建一个字典来保存相同点数的卡牌
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

        //根据手牌长度来决定类型,先初步对5张以内的类型进行判断
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
                if(PlayCards[0].ValueWeight>=14 && PlayCards[1].ValueWeight >= 15)//王炸
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
                    PlayCards[2].ValueWeight == PlayCards[3].ValueWeight)//炸弹
                {
                    cardType = CardType.bomb;
                    isConformRule = true;
                }
                else if ((PlayCards[0].ValueWeight == PlayCards[1].ValueWeight && 
                    PlayCards[1].ValueWeight == PlayCards[2].ValueWeight &&
                    PlayCards[2].ValueWeight != PlayCards[3].ValueWeight) || 
                    PlayCards[0].ValueWeight != PlayCards[1].ValueWeight &&
                    PlayCards[1].ValueWeight == PlayCards[2].ValueWeight &&
                    PlayCards[2].ValueWeight == PlayCards[3].ValueWeight)//三带一，两种情况，前面三张相等，带最后一张,或第一张不相等，后面三张带第一张
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

                if (hasFour && hasOne) //四带一
                {
                    cardType = CardType.fourAndOne;
                    isConformRule = true;
                }
                else if (hasThree && hasTwo) //三带二
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

                    if(hasFour && hasTwo)//四带二
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

                if(hasOne && hasTwo == false && hasThree == false && hasFour == false)//顺子
                {
                    isStraight = true;
                    for (int i = 0; i < cardNum-1; i++)
                    {
                        if((PlayCards[i+1].ValueWeight - PlayCards[i].ValueWeight != 1) || PlayCards[i].ValueWeight>12)//后一张牌比前一张牌大1，且不能为2或者大小王
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

    public bool CompareCard() //true比较结果成功，出的牌比上一名玩家大，false比较结果失败，出的牌比上一名玩家小
    {
        if(cardType != LastCardType)
        {
            return false;
        }

        Debug.Log("cardType=" + cardType);
        //对不同类型的牌做判断
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

    public void sendPlayerInfoServer(PlayerInfo playerInfo) //发送出牌的数据到服务端
    {
        NetManager.Instance.SendMessageToServer_net(41, playerInfo);
    }

    public void SendLastCardListToServer() //将接收到的数据直接发送给下一个客户端
    {
        CardList cardList = new CardList();
        cardList.cards = new List<Card>(NetManager.Instance.LastCardContainer.cards);
        NetManager.Instance.SendMessageToServer(43, cardList);
    }
}
