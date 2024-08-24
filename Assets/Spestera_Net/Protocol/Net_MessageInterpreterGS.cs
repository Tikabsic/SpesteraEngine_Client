using Google.Protobuf;
using System;
using UnityEngine;

public class Net_MessageInterpreterGS
{
    public static event Action OnClientLogin;

    public static event Action<PlayerInitialData> OnOtherClientLogin;
    public static event Action<uint> OnOtherClientLogout;
    public static event Action<string, bool> OnLoginStateMessage;


    public void InterpretMessage(byte[] data, int length)
    {
        NetworkManager._syncContext.Post(_ =>
        {
            try
            {
                GSWrapper wrapper = new GSWrapper();

                wrapper = GSWrapper.Parser.ParseFrom(data, 0, length);
                if (wrapper != null)
                {

                    switch (wrapper.Type)
                    {
                        case GSWrapper.Types.MessageType.Clientlogout:
                            HandleClientLogout(wrapper.Payload);
                            break;
                        //case GSWrapper.Types.MessageType.Clientlogin:
                        //    Debug.Log("Client login");
                        //    HandleClientLogin(wrapper.Payload);
                        //    break;
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

    public void HandleWrapper(GSWrapper wrapper)
    {

        NetworkManager._syncContext.Post(_ =>
        {
            try
            {
                if (wrapper != null)
                {

                    switch (wrapper.Type)
                    {
                        case GSWrapper.Types.MessageType.Assignid:
                            HandleAssignId(wrapper.Payload);
                            break;
                        case GSWrapper.Types.MessageType.Clientlogout:
                            HandleClientLogout(wrapper.Payload);
                            break;
                        //case GSWrapper.Types.MessageType.Clientlogin:
                        //    Debug.Log("Client login");
                        //    HandleClientLogin(wrapper.Payload);
                        //
                        case GSWrapper.Types.MessageType.Loginrequestresult:
                            LoginRequestResult result = LoginRequestResult.Parser.ParseFrom(wrapper.Payload);
                            OnLoginStateMessage?.Invoke(result.Message, result.ValidationResult);
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

    private void HandleAssignId(ByteString payload)
    {
        var id = AssignId.Parser.ParseFrom(payload);
        NetworkCredits.SetPlayerId((uint)id.String);
        OnClientLogin?.Invoke();
    }

    private void HandleClientLogin(ByteString payload)
    {
        //Debug.Log("New client logged in");
        //ClientLogin newClientLoginData = ClientLogin.Parser.ParseFrom(payload);
        //PlayerInitialData newClientInitialData = newClientLoginData.PlayerId;
        //OnOtherClientLogin?.Invoke(newClientInitialData);
    }

    private void HandleClientLogout(ByteString payload)
    {
        var data = ClientLogout.Parser.ParseFrom(payload);
        OnOtherClientLogout?.Invoke(data.PlayerId);
    }
}