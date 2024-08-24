using Google.Protobuf;
using System;
using UnityEngine;

public class Net_MessageInterpreterZS
{
    public static event Action<PlayerInitialData> OnPlayerDataRecived;
    public static event Action OnPlayerInitialized;
    public static event Action<uint> OnOtherPlayerLogout;
    public static event Action<PlayerInitialData> OnOtherPlayerLogin;
    public static event Action<WorldData> OnWorldDataRecived;

    public void InterpretMessage(byte[] data, int length)
    {
        NetworkManager._syncContext.Post(_ =>
        {
            try
            {
                ZSWrapper wrapper = new ZSWrapper();

                wrapper = ZSWrapper.Parser.ParseFrom(data, 0, length);
                if (wrapper != null)
                {

                    switch (wrapper.Type)
                    {
                        case ZSWrapper.Types.MessageType.Heartbeat:
                            Heartbeat hb = Heartbeat.Parser.ParseFrom(wrapper.Payload);
                            Net_HeartbeatHandler.Instance.HandleMessage(hb);
                            break;
                        case ZSWrapper.Types.MessageType.Worlddata:
                            HandleWorldData(wrapper.Payload);
                            break;
                        case ZSWrapper.Types.MessageType.Playerinitialdata:
                            HandleInitialData(wrapper.Payload);
                            break;
                        case ZSWrapper.Types.MessageType.Playerout:
                            HandlePlayerLogout(wrapper.Payload);
                            break;
                        case ZSWrapper.Types.MessageType.Playerin:
                            Debug.Log("Client login");
                            HandlePlayerIn(wrapper.Payload);
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

    public void HandleWrapper(ZSWrapper wrapper)
    {
        NetworkManager._syncContext.Post(_ =>
        {
            try
            {
                if (wrapper != null)
                {

                    switch (wrapper.Type)
                    {
                        case ZSWrapper.Types.MessageType.Heartbeat:
                            Heartbeat hb = Heartbeat.Parser.ParseFrom(wrapper.Payload);
                            Net_HeartbeatHandler.Instance.HandleMessage(hb);
                            break;
                        case ZSWrapper.Types.MessageType.Playerinitialdata:
                            HandleInitialData(wrapper.Payload);
                            break;
                        case ZSWrapper.Types.MessageType.Playerout:
                            HandlePlayerLogout(wrapper.Payload);
                            break;
                        case ZSWrapper.Types.MessageType.Playerin:
                            Debug.Log("Client login");
                            HandlePlayerIn(wrapper.Payload);
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

    private void HandlePlayerIn(ByteString payload)
    {
        Debug.Log("New player logged in");
        PlayerIn newClientLoginData = PlayerIn.Parser.ParseFrom(payload);
        PlayerInitialData newClientInitialData = newClientLoginData.Data;
        OnOtherPlayerLogin?.Invoke(newClientInitialData);
    }

    private void HandlePlayerLogout(ByteString payload)
    {
        var data = ClientLogout.Parser.ParseFrom(payload);
        OnOtherPlayerLogout?.Invoke(data.PlayerId);
    }

    private void HandleInitialData(ByteString payload)
    {
        Debug.Log("initial data recived");
        var playerInitialData = PlayerInitialData.Parser.ParseFrom(payload);
        OnPlayerDataRecived?.Invoke(playerInitialData);
        OnPlayerInitialized?.Invoke();
    }

    private void HandleWorldData(ByteString payload)
    {
        var worldData = WorldData.Parser.ParseFrom(payload);
        OnWorldDataRecived?.Invoke(worldData);
    }
}
