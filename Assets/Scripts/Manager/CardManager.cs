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

        // ��ȡ���ƶ����ϵ� CardBehavior ���
        CardPrefab cardBehavior = cardObject.GetComponent<CardPrefab>();

        // ʹ�� CardBehavior ��������ÿ���
        //CardPrefab.SetCard(cardData);
    }

}
