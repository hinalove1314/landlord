using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class CardManager : MonoBehaviour
{
    private UIManager m_UIManager;

    public List<Card> allCards = new List<Card>();//����������
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

}
