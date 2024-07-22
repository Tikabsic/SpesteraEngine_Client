using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    //Basic info
    public string ServerIP = "127.0.0.1";
    public int ServerPort_TCP = 7171;
    public int ServerPort_UDP = 7172;
    public static SynchronizationContext _syncContext;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        _syncContext = SynchronizationContext.Current;
        Net_ConnectionHandler.Instance.BeginConnect(ServerIP, ServerPort_TCP, ServerPort_UDP);
    }

    private void OnApplicationClose()
    {
        Net_ConnectionHandler.Instance.CloseConnection();
    }
}