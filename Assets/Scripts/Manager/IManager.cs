using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManager
{
    //初始化
    void Init(params object[] managers);
    //帧更新
    void Update();
    //销毁时调用(主要是释放数据等使用)
    void Destroy();
    //游戏结束

}
