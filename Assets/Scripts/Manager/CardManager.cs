using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class CardManager : MonoBehaviour
{
    private UIManager m_UIManager;

    public List<Card> allCards = new List<Card>();//代表所有牌
    public static CardManager Instance { get; private set; }

    public GameObject cardPrefab;
    public Transform cardPanel;

    public void Init(params object[] managers)
    {
        m_UIManager = managers[0] as UIManager;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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

}
