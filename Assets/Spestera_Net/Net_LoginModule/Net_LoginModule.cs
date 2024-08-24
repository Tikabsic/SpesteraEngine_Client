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

        GSWrapper wrapper = new GSWrapper();
        wrapper.Type = GSWrapper.Types.MessageType.Requestlogin;
        wrapper.Payload = loginRequest.ToByteString();

        Net_ConnectionHandler.Instance.SendSpesteraMessage_GameServer(wrapper, false);

        _loginField.text = "";
        _passwordField.text = "";
    }

    private void ValidationResult(string message, bool validationresult)
    {
        if (validationresult)
        {
            Debug.Log(message);
        }
        else
        {
            Debug.Log(message);
            Net_ConnectionHandler.Instance.CloseConnection();
        }
    }

}
