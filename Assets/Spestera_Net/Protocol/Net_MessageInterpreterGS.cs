using Google.Protobuf;
using System;
using UnityEngine;

public class Net_MessageInterpreterGS
{
    public static event Action OnClientLogin;

    public static event Action<PlayerInitialData> OnOtherClientLogin;
    public static event Action<uint> OnOtherClientLogout;
    public static event Action<string> OnLoginStateMessage;


    public void InterpretMessage(byte[] data, int length)
    {
        NetworkManager._syncContext.Post(_ =>
        {
            try
            {
                GSWrapperResponse wrapper = new GSWrapperResponse();

                wrapper = GSWrapperResponse.Parser.ParseFrom(data, 0, length);
                if (wrapper != null)
                {

                    switch (wrapper.ResponseCase)
                    {
                        case GSWrapperResponse.ResponseOneofCase.ConnectionResponse:
                            HandleConnectionResponseWrapper(wrapper.ConnectionResponse);
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

    public void HandleWrapper(GSWrapperResponse wrapper)
    {

        NetworkManager._syncContext.Post(_ =>
        {
            try
            {
                if (wrapper != null)
                {
                    switch (wrapper.ResponseCase)
                    {
                        case GSWrapperResponse.ResponseOneofCase.ConnectionResponse:
                            HandleConnectionResponseWrapper(wrapper.ConnectionResponse);
                            break;
                        default:
                            Debug.Log("Unknown wrapper type");
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

    #region ConnectionResponseWrapper

    private void HandleConnectionResponseWrapper(ConnectionResponseWrapper wrapper)
    {
        switch (wrapper.ActionCase)
        {
            case ConnectionResponseWrapper.ActionOneofCase.AssignId:
                HandleAssignId(wrapper.AssignId);
                break;
            case ConnectionResponseWrapper.ActionOneofCase.LoginResult:
                OnLoginStateMessage?.Invoke(wrapper.LoginResult.Message);
                break;
            case ConnectionResponseWrapper.ActionOneofCase.LoginResponse:

                break;
            default:
                Debug.Log("Unknown action case...");
                break;
        }

    }


    private void HandleAssignId(AssignId id)
    {
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
    #endregion

}