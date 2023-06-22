using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    private UIManager m_UIManager;

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

    public void AddCard(Card cardData)
    {
        GameObject cardObject = Instantiate(cardPrefab, cardPanel.transform);

        // 获取卡牌对象上的 CardBehavior 组件
        CardPrefab cardBehavior = cardObject.GetComponent<CardPrefab>();

        // 使用 CardBehavior 组件来设置卡牌
        //CardPrefab.SetCard(cardData);
    }

}
