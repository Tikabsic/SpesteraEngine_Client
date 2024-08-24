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
    public int ServerPort_GS = 7171;
    public int ServerPort_ZS = 7175;
    public static SynchronizationContext _syncContext;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        _syncContext = SynchronizationContext.Current;
        //Net_ConnectionHandler.Instance.BeginConnectToGameServer(ServerIP, ServerPort_GS, ServerPort_ZS);
    }

    private void OnApplicationQuit()
    {
        Net_ConnectionHandler.Instance.LogoutFromGame();
    }
}