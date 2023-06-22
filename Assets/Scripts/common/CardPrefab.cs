using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPrefab : MonoBehaviour
{
    public UIManager m_UIManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCard(Card cardData)
    {
        // 获取牌对象上的 Image 组件
        Image cardImage = GetComponent<Image>();

        // 根据 cardData 来构造图片的路径
        string imagePath = "Poker/" + cardData.suit + cardData.value;

        if (cardData.value == "big_joker")
        {
            imagePath = "Poker/" + "LJoker";
        }else if (cardData.value == "little_joker")
        {
            imagePath = "Poker/" + "SJoker";
        }

        // 使用 Resources.Load 函数来加载图片
        Sprite newSprite = Resources.Load<Sprite>(imagePath);

        // 更改 Image 组件的图片
        cardImage.sprite = newSprite;
    }
}
