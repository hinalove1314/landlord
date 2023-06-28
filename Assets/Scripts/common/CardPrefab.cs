using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPrefab : MonoBehaviour
{
    public UIManager m_UIManager;
    public Card m_cardData; //卡牌数据
    public bool isSelected = false; // 新增，用来记录牌是否被选中
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
        //给自身的卡牌数据赋值
        m_cardData = cardData;

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

    void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(0)) // 检查是否是左键释放
        {
            isSelected = !isSelected; // 切换选中状态
            if (isSelected)
            {
                // 如果选中，让牌弹起
                transform.position += new Vector3(0, 0.3f, 0);
            }
            else
            {
                // 如果取消选中，让牌回到原位
                transform.position -= new Vector3(0, 0.3f, 0);
            }
        }
    }
}
