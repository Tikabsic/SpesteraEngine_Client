using Google.Protobuf;
using System;
using UnityEngine;

public class Net_MessageInterpreter
{
    public static event Action<PlayerInitialData> OnPlayerDataRecived;
    public static event Action<uint> OnOtherPlayerLogout;

    public void InterpretMessage(byte[] data, int length)
    {
        try
        {
            Wrapper wrapper = Wrapper.Parser.ParseFrom(data, 0, length);
            if (wrapper != null)
            {
                switch (wrapper.Type)
                {
                    case Wrapper.Types.MessageType.Response:
                        HandleResponse(wrapper.Payload);
                        break;
                    case Wrapper.Types.MessageType.Playerinitialdata:

                        break;
                    default:
                        try
                        {
                            Heartbeat heartbeat = Heartbeat.Parser.ParseFrom(data, 0, length);
                            if (heartbeat != null)
                            {
                                Net_HeartbeatHandler.Instance.HandleMessage(heartbeat);
                                return;
                            }
                        }
                        catch (InvalidProtocolBufferException ex)
                        {
                            Debug.LogError($"Error parsing as Heartbeat: {ex.Message}");
                        }
                        break;
                }
                return;
            }
        }
        catch (InvalidProtocolBufferException ex)
        {
            Debug.LogError($"Error parsing as Wrapper: {ex.Message}");
        }
        Debug.Log("Unknown message format");
    }

    public void HandleWrapper(Wrapper wrapper)
    {
        try
        {
            if (wrapper != null)
            {
                switch (wrapper.Type)
                {
                    case Wrapper.Types.MessageType.Response:
                        Debug.Log("response");
                        HandleResponse(wrapper.Payload);
                        break;
                    case Wrapper.Types.MessageType.Playerinitialdata:
                        HandleInitialData(wrapper.Payload);
                        break;
                    case Wrapper.Types.MessageType.Clientlogout:
                        Debug.Log("ClientLogout");
                        //HandleClientLogout(wrapper.Payload);
                        break;
                    default:
                        break;
                }
                return;
            }
        }
        catch (InvalidProtocolBufferException ex)
        {
            Debug.LogError($"Error parsing as Wrapper: {ex.Message}");
        }
        Debug.Log("Unknown message format");
    }
    private void HandleResponse(ByteString payload)
    {

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
    }
}