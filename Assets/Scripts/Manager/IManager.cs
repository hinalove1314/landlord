using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManager
{
    //��ʼ��
    void Init(params object[] managers);
    //֡����
    void Update();
    //����ʱ����(��Ҫ���ͷ����ݵ�ʹ��)
    void Destroy();
    //��Ϸ����

}
