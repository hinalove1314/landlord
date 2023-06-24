using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public static Hand instance;
    public GameObject cardPrefab;  // 指定卡牌预制体

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
        // 使用 Instantiate 函数实例化一个新的牌对象
        GameObject cardObject = Instantiate(cardPrefab, transform);  // 使新创建的牌对象成为 cardPanel 的子对象

        cardObject.SetActive(true);
        // 设置牌对象的属性，例如更改牌的图片等
        CardPrefab cardComponent = cardObject.GetComponent<CardPrefab>();
        cardComponent.SetCard(cardData);
    }

    public void ClearAllCards()
    {
        // 遍历卡片列表，销毁每一个 GameObject
        foreach (Transform child in transform)
        {
            if(child.name == "Card")
            {
                continue;
            }
            GameObject.Destroy(child.gameObject);
        }
    }
}
