using Google.Protobuf;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;


public class Net_ConnectionHandler : Singleton<Net_ConnectionHandler>
{
    //Events Base
    public event Action OnClientLogout;
    public event Action OnClientLogin;

    //Events UI 
    public event Action<int> OnBandwidthRecivedUDP;
    public event Action<int> OnBandwidthRecivedTCP;


    //Base props
    private string _serverIp = "";
    private int _serverPort_UDP = 0;
    private int _serverPort_TCP = 0;
    private byte[] buffer = new byte[512 * 1024];

    //TCP client
    private NetworkStream stream;
    private System.Net.Sockets.TcpClient _tcpClient;
    private bool _isConnected;

    //UDP client
    private UdpClient _udpClient;

    //Message interpreter
    private Net_MessageInterpreter _messageInterpreter;

    public async void BeginConnect(string serverIp, int tcpport, int udpport)
    {
        // Preload basic classes
        _serverIp = serverIp;
        _serverPort_UDP = udpport;
        _serverPort_TCP = tcpport;
        _messageInterpreter = new Net_MessageInterpreter();

        // TCP client preparation
        _tcpClient = new TcpClient();

        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        _tcpClient.Client = socket;

        await _tcpClient.ConnectAsync(serverIp, tcpport);
        stream = _tcpClient.GetStream();
        _ = Task.Run(() => ReceiveSpesteraMessages_TCP());

        if (_tcpClient.Connected)
        {
            _isConnected = true;
            OnClientLogin?.Invoke();
        }
    }

    public void ConnectToUDPServer()
    {
        _udpClient = new UdpClient(0);
        _ = Task.Run(() => ReceiveSpesteraMessages_UDP());

        SendSpesteraLoginRequest_UDP();
    }

    #region TCP Messages
    private async Task ReceiveSpesteraMessages_TCP()
    {
        while (true)
        {
            if (stream.DataAvailable)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    try
                    {
                        Wrapper wrapper = Wrapper.Parser.ParseFrom(buffer, 0, bytesRead);
                        _messageInterpreter.HandleWrapper(wrapper);
                    }
                    catch (InvalidProtocolBufferException ex)
                    {
                        byte[] decompressedData = TryDecompress(buffer);
                        if (decompressedData != null)
                        {
                            _messageInterpreter.InterpretMessage(decompressedData, decompressedData.Length);
                        }
                        else
                        {
                            Debug.LogError("Failed to parse data as Wrapper and decompress.");

                        }
                    }
                }
            }

            await Task.Delay(1);
        }
    }

    private byte[] TryDecompress(byte[] compressedData)
    {
        // Spróbuj zdekompresowaæ dane
        try
        {
            return ByteCompressor.DecompressData(compressedData);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async void SendSpesteraMessage_TCP(Wrapper wrapper)
    {
        try
        {
            if (_tcpClient == null)
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(IPAddress.Parse(_serverIp), _serverPort_TCP);
            }

            // Przygotowanie danych do wys³ania
            byte[] data = wrapper.ToByteArray();

            // Przes³anie danych przez po³¹czenie TCP
            await _tcpClient.GetStream().WriteAsync(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wyst¹pi³ b³¹d podczas wysy³ania danych: {ex.Message}");
        }
    }
    #endregion

    #region UDP Messages
    private async Task ReceiveSpesteraMessages_UDP()
    {
        while (true)
        {
            UdpReceiveResult result = await _udpClient.ReceiveAsync();
            byte[] decompressedData = ByteCompressor.DecompressData(result.Buffer);
            OnBandwidthRecivedUDP?.Invoke(result.Buffer.Length);
            _messageInterpreter.InterpretMessage(decompressedData, decompressedData.Length);
        }
    }

    public async void SendSpesteraMessage_UDP(Wrapper wrapper)
    {
        var data = wrapper.ToByteArray();
        try
        {
            await _udpClient.SendAsync(data, data.Length, _serverIp, _serverPort_UDP);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending UDP message: {ex.Message}");
        }
    }

    public async void SendSpesteraLoginRequest_UDP()
    {
        Wrapper requestWrapper = new Wrapper();
        RequestLogin request = new RequestLogin();
        request.PlayerId = NetworkCredits.PlayerId;
        requestWrapper.Type = Wrapper.Types.MessageType.Requestlogin;
        requestWrapper.Payload = ByteString.CopyFrom(request.ToByteArray());

        var data = requestWrapper.ToByteArray();

        try
        {
            await _udpClient.SendAsync(data, data.Length, _serverIp, _serverPort_UDP);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending UDP message: {ex.Message}");
        }
    }
    #endregion

    #region Logout Messages

    private void SendSpesteraLogoutMessage_Both()
    {
        ClientLogout logoutMessage = new ClientLogout();
        logoutMessage.PlayerId = NetworkCredits.PlayerId;

        Wrapper logoutWrapper = new Wrapper();
        logoutWrapper.Type = Wrapper.Types.MessageType.Clientlogout;
        logoutWrapper.Payload = logoutMessage.ToByteString();

        SendSpesteraMessage_TCP(logoutWrapper);
        SendSpesteraMessage_UDP(logoutWrapper);
    }

    #endregion

    public void CloseConnection()
    {
        OnClientLogout?.Invoke();
        SendSpesteraLogoutMessage_Both();
        _tcpClient.Close();
        _udpClient.Close();
    }

}
