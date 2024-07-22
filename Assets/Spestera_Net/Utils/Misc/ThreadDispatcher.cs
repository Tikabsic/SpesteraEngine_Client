using System;
using System.Collections.Concurrent;
using UnityEngine;

public class ThreadDispatcher : MonoBehaviour
{
    private static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();

    private static ThreadDispatcher _instance;

    public static ThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                var gameObject = new GameObject("ThreadDispatcher");
                _instance = gameObject.AddComponent<ThreadDispatcher>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }

    public static void Enqueue(Action action)
    {
        _executionQueue.Enqueue(action);
    }

    private void Update()
    {
        while (_executionQueue.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }
}
