using Google.Protobuf;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Net_ConnectionHandler : Singleton<Net_ConnectionHandler>
{
    //Events Base
    public event Action OnClientLogout;
    public event Action OnPlayerLogout;

    //Events UI 
    public event Action<int> OnBandwidthRecivedZS;
    public event Action<int> OnBandwidthRecivedGS;

    //Base props
    private string _serverIp = "";
    private int _serverPort_GS = 0;
    private int _serverPort_ZS = 0;
    private byte[] _gsBuffer = new byte[512 * 1024];
    private byte[] _zsBuffer = new byte[512 * 1024];

    //GS client
    private NetworkStream _streamGS;
    private System.Net.Sockets.TcpClient _tcpClientGS;
    private CancellationTokenSource _gsCancellationToken;

    //ZS client
    private NetworkStream _streamZS;
    private System.Net.Sockets.TcpClient _tcpClientZS;

    //Logic gates
    private bool _isDisconnecting = false;

    //Message interpreter
    private Net_MessageInterpreterGS _messageInterpreterGS;
    private Net_MessageInterpreterZS _messageInterpreterZS;

    System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

    public async void ConnectToGameServer(string serverIp, int gsport, int zsport)
    {
        // Preload basic classes
        _serverIp = serverIp;
        _serverPort_GS = gsport;
        _serverPort_ZS = zsport;
        _messageInterpreterGS = new Net_MessageInterpreterGS();

        // TCP client preparation
        _tcpClientGS = new TcpClient();
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        _tcpClientGS.Client = socket;
        _gsCancellationToken = new CancellationTokenSource();

        await _tcpClientGS.ConnectAsync(_serverIp, _serverPort_GS);
        _streamGS = _tcpClientGS.GetStream();
        _ = Task.Run(() => ReceiveSpesteraMessages_GameServer(_gsCancellationToken.Token));
    }

    public async void ConnectToZoneServerServer()
    {
        _messageInterpreterZS = new Net_MessageInterpreterZS();

        // TCP client preparation
        _tcpClientZS = new TcpClient();

        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        _tcpClientZS.Client = socket;

        await _tcpClientZS.ConnectAsync(_serverIp, _serverPort_ZS);
        _streamZS = _tcpClientZS.GetStream();
        _ = Task.Run(() => ReceiveSpesteraMessages_ZoneServer());
    }

    #region GameServer Messages
    private async Task ReceiveSpesteraMessages_GameServer(CancellationToken token)
    {
        
        while(!token.IsCancellationRequested)
        {
            try
            {
                var bytesRead = await _streamGS.ReadAsync(_gsBuffer, 0, _gsBuffer.Length);
                if (bytesRead > 0)
                {
                    OnBandwidthRecivedGS?.Invoke(bytesRead);

                    try
                    {
                        GSWrapperResponse wrapper = GSWrapperResponse.Parser.ParseFrom(_gsBuffer, 0, bytesRead);
                        _messageInterpreterGS.HandleWrapper(wrapper);
                    }
                    catch (InvalidProtocolBufferException ex)
                    {
                    }

                    try
                    {
                        byte[] decompressedData = TryDecompress(_gsBuffer);
                        if (decompressedData != null)
                        {
                            _messageInterpreterGS.InterpretMessage(decompressedData, decompressedData.Length);
                        }
                    }
                    catch (InvalidProtocolBufferException ex)
                    {
                        Debug.LogError($"Decompression error: {ex.Message}");
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.LogError($"IO Exception: {ex.Message}");
                break;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: {ex.Message}");
                break;
            }

            await Task.Delay(1);
        }
    }

    private byte[] TryDecompress(byte[] compressedData)
    {
        try
        {
            return ByteCompressor.DecompressData(compressedData);
        }
        catch (Exception ex)
        {
            return null;
        }
    }



    public async void SendSpesteraMessage_GameServer(GSWrapperRequest wrapper, bool isCompressed)
    {
        try
        {
            if (isCompressed)
            {
                var compressedData = ByteCompressor.CompressData(wrapper.ToByteArray());
                await _tcpClientGS.GetStream().WriteAsync(compressedData, 0, compressedData.Length);
                Debug.Log($"sent compressed data {compressedData.Length} <- lenght");
            }
            else
            {
                byte[] data = wrapper.ToByteArray();
                await _tcpClientGS.GetStream().WriteAsync(data, 0, data.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while sending message : {ex.Message}");
        }
    }
    #endregion

    #region UDP Messages
    private async Task ReceiveSpesteraMessages_ZoneServer()
    {
        while (!_isDisconnecting)
        {
            try
            {
                var bytesRead = await _streamZS.ReadAsync(_zsBuffer, 0, _zsBuffer.Length);
                if (bytesRead > 0)
                {
                    OnBandwidthRecivedGS?.Invoke(bytesRead);

                    try
                    {
                        ZSWrapper wrapper = ZSWrapper.Parser.ParseFrom(_zsBuffer, 0, bytesRead);
                        _messageInterpreterZS.HandleWrapper(wrapper);
                    }
                    catch (InvalidProtocolBufferException ex)
                    {

                    }

                    try
                    {
                        byte[] decompressedData = TryDecompress(_zsBuffer);
                        if (decompressedData != null)
                        {
                            _messageInterpreterZS.InterpretMessage(decompressedData, decompressedData.Length);
                        }
                        else
                        {
                        }
                    }
                    catch (InvalidProtocolBufferException ex)
                    {
                        Debug.LogError($"Decompression error: {ex.Message}");
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.LogError($"IO Exception: {ex.Message}");
                break;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: {ex.Message}");
                break;
            }

            await Task.Delay(1);
        }
    }
    int msgcounter = 0;
    public async void SendSpesteraMessage_ZoneServer(ZSWrapper wrapper, bool isCompressed)
    {

        try
        {
            if (isCompressed)
            {
                var compressedData = ByteCompressor.CompressData(wrapper.ToByteArray());
                await _tcpClientZS.GetStream().WriteAsync(compressedData, 0, compressedData.Length);
            }
            else
            {
                byte[] data = wrapper.ToByteArray();
                await _tcpClientZS.GetStream().WriteAsync(data, 0, data.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while sending message : {ex.Message}");
        }
    }

    #endregion

    #region Logout Messages

    private void SendSpesteraLogoutMessage()
    {
        ClientLogout logoutMessage = new ClientLogout();
        PlayerOut logoutPlayer = new PlayerOut();

        logoutMessage.PlayerId = NetworkCredits.PlayerId;
        logoutPlayer.PlayerId = NetworkCredits.PlayerId;

        GSWrapperRequest logoutWrapper = new GSWrapperRequest();
        ConnectionRequestWrapper connectionWrapper = new ConnectionRequestWrapper();
        logoutWrapper.ConnectionRequest = connectionWrapper;
        logoutWrapper.ConnectionRequest.ClientLogout = logoutMessage;

        ZSWrapper playerLogoutWrapper = new ZSWrapper();
        playerLogoutWrapper.Type = ZSWrapper.Types.MessageType.Playerout;
        playerLogoutWrapper.Payload = logoutPlayer.ToByteString();

        SendSpesteraMessage_GameServer(logoutWrapper, false);
        SendSpesteraMessage_ZoneServer(playerLogoutWrapper, false);
    }

    #endregion

    public void CloseConnection()
    {
        if (_tcpClientGS.Connected)
        {
            _tcpClientGS.Close();
        }
        if (_tcpClientZS.Connected)
        {
            _tcpClientZS.Close();
        }
    }

    public void LogoutFromGame()
    {
        OnClientLogout?.Invoke();
        _isDisconnecting = true;
        SendSpesteraLogoutMessage();
        _tcpClientGS.Close();
        _tcpClientZS.Close();
    }

}
