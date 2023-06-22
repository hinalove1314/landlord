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
        // ��ȡ�ƶ����ϵ� Image ���
        Image cardImage = GetComponent<Image>();

        // ���� cardData ������ͼƬ��·��
        string imagePath = "Poker/" + cardData.suit + cardData.value;

        if (cardData.value == "big_joker")
        {
            imagePath = "Poker/" + "LJoker";
        }else if (cardData.value == "little_joker")
        {
            imagePath = "Poker/" + "SJoker";
        }

        // ʹ�� Resources.Load ����������ͼƬ
        Sprite newSprite = Resources.Load<Sprite>(imagePath);

        // ���� Image �����ͼƬ
        cardImage.sprite = newSprite;
    }
}
