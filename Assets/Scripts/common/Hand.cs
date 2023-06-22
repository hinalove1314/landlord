using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public static Hand instance;
    public GameObject cardPrefab;  // ָ������Ԥ����

    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("init hand instance");
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddCard(Card cardData)
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