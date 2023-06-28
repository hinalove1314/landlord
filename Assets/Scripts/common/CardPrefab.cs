using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPrefab : MonoBehaviour
{
    public UIManager m_UIManager;
    public Card m_cardData; //��������
    public bool isSelected = false; // ������������¼���Ƿ�ѡ��
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
        //������Ŀ������ݸ�ֵ
        m_cardData = cardData;

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

    void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(0)) // ����Ƿ�������ͷ�
        {
            isSelected = !isSelected; // �л�ѡ��״̬
            if (isSelected)
            {
                // ���ѡ�У����Ƶ���
                transform.position += new Vector3(0, 0.3f, 0);
            }
            else
            {
                // ���ȡ��ѡ�У����ƻص�ԭλ
                transform.position -= new Vector3(0, 0.3f, 0);
            }
        }
    }
}
