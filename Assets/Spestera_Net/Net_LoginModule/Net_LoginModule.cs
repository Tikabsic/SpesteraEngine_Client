using Google.Protobuf;
using TMPro;
using UnityEngine;

public class Net_LoginModule : MonoBehaviour
{
    [SerializeField] private NetworkManager _manager;

    //GUI
    [SerializeField] private TMP_InputField _loginField;
    [SerializeField] private TMP_InputField _passwordField;

    private void Start()
    {
        Net_MessageInterpreterGS.OnClientLogin += SendLoginAndPasswordToVerify;
        Net_MessageInterpreterGS.OnLoginStateMessage += ValidationResult;
    }

    public void OnLoginButton()
    {
        Net_ConnectionHandler.Instance.ConnectToGameServer(_manager.ServerIP, _manager.ServerPort_GS, _manager.ServerPort_ZS);
    }

    private void SendLoginAndPasswordToVerify()
    {
        RequestLogin loginRequest = new RequestLogin();
        loginRequest.AccountName = _loginField.text;
        loginRequest.Password = _passwordField.text;

        GSWrapperRequest wrapper = new GSWrapperRequest();
        ConnectionRequestWrapper connectionWrapper = new ConnectionRequestWrapper();
        wrapper.ConnectionRequest = connectionWrapper;
        wrapper.ConnectionRequest.RequestLogin = loginRequest;

        Net_ConnectionHandler.Instance.SendSpesteraMessage_GameServer(wrapper, false);

        _loginField.text = "";
        _passwordField.text = "";
    }

    private void ValidationResult(string message)
    {
        Debug.Log(message);
    }

}
