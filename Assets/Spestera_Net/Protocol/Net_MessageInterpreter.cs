using Google.Protobuf;
using System;
using UnityEngine;

public class Net_MessageInterpreter
{
    public static event Action<PlayerInitialData> OnPlayerDataRecived;
    public static event Action OnPlayerLogin;
    public static event Action<uint> OnOtherPlayerLogout;
    public static event Action<PlayerInitialData> OnOtherPlayerLogin;
    public static event Action<WorldData> OnWorldDataRecived;

    public void InterpretMessage(byte[] data, int length)
    {
        NetworkManager._syncContext.Post(_ =>
        {
            try
            {
                Wrapper wrapper = new Wrapper();
                wrapper = Wrapper.Parser.ParseFrom(data, 0, length);
                if (wrapper != null)
                {

                    switch (wrapper.Type)
                    {
                        case Wrapper.Types.MessageType.Heartbeat:
                            Heartbeat hb = Heartbeat.Parser.ParseFrom(wrapper.Payload);
                            Net_HeartbeatHandler.Instance.HandleMessage(hb);
                            break;
                        case Wrapper.Types.MessageType.Worlddata:
                            HandleWorldData(wrapper.Payload);
                            break;
                        case Wrapper.Types.MessageType.Response:
                            HandleResponse(wrapper.Payload);
                            break;
                        case Wrapper.Types.MessageType.Playerinitialdata:
                            HandleInitialData(wrapper.Payload);
                            break;
                        case Wrapper.Types.MessageType.Clientlogout:
                            HandleClientLogout(wrapper.Payload);
                            break;
                        case Wrapper.Types.MessageType.Clientlogin:
                            Debug.Log("Client login");
                            HandleClientLogin(wrapper.Payload);
                            break;
                        default:
                            Debug.Log("Unknown message type");
                            break;
                    }
                    return;
                }
            }
            catch (InvalidProtocolBufferException ex)
            {
                Debug.LogError($"Error parsing as Wrapper: {ex.Message}");
            }
        }, null);
    }

    public void HandleWrapper(Wrapper wrapper)
    {

        NetworkManager._syncContext.Post(_ =>
        {
            try
            {
                if (wrapper != null)
                {

                    switch (wrapper.Type)
                    {
                        case Wrapper.Types.MessageType.Heartbeat:
                            Heartbeat hb = Heartbeat.Parser.ParseFrom(wrapper.Payload);
                            Net_HeartbeatHandler.Instance.HandleMessage(hb);
                            break;
                        case Wrapper.Types.MessageType.Response:
                            HandleResponse(wrapper.Payload);
                            break;
                        case Wrapper.Types.MessageType.Playerinitialdata:
                            HandleInitialData(wrapper.Payload);
                            break;
                        case Wrapper.Types.MessageType.Clientlogout:
                            HandleClientLogout(wrapper.Payload);
                            break;
                        case Wrapper.Types.MessageType.Clientlogin:
                            Debug.Log("Client login");
                            HandleClientLogin(wrapper.Payload);
                            break;
                        default:
                            Debug.Log("Unknown message type");
                            break;
                    }
                    return;
                }
            }
            catch (InvalidProtocolBufferException ex)
            {
                Debug.Log($"Error parsing as Wrapper: {ex.Message}");
            }
            Debug.Log("Unknown message format");
        }, null);
    }
    private void HandleResponse(ByteString payload)
    {

    }

    private void HandleClientLogin(ByteString payload)
    {
        Debug.Log("New player logged in");
        ClientLogin newClientLoginData = ClientLogin.Parser.ParseFrom(payload);
        PlayerInitialData newClientInitialData = newClientLoginData.PlayerData;
        OnOtherPlayerLogin?.Invoke(newClientInitialData);
    }

    private void HandleClientLogout(ByteString payload)
    {
        var data = ClientLogout.Parser.ParseFrom(payload);
        OnOtherPlayerLogout?.Invoke(data.PlayerId);
    }

    private void HandleInitialData(ByteString payload)
    {
        var playerInitialData = PlayerInitialData.Parser.ParseFrom(payload);
        NetworkCredits.SetPlayerId(playerInitialData.PlayerId);
        OnPlayerDataRecived?.Invoke(playerInitialData);
        OnPlayerLogin?.Invoke();
    }

    private void HandleWorldData(ByteString payload)
    {
        var worldData = WorldData.Parser.ParseFrom(payload);
        OnWorldDataRecived?.Invoke(worldData);
    }
}