using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher instance;
    private readonly Queue<System.Action> actionQueue = new Queue<System.Action>();
    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        while (actionQueue.Count > 0)
        {
            actionQueue.Dequeue().Invoke();
        }
    }

    //Enqueue 函数的作用是将一个 Action 添加到队列中。在 Update 函数中，遍历并执行队列中的所有 Action
    public void Enqueue(System.Action action)
    {
        actionQueue.Enqueue(action);
    }

    public void printInstance()
    {
        Debug.Log("printInstance");
    }
}

