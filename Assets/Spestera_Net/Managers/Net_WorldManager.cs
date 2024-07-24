using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_WorldManager : MonoBehaviour
{

    private void Awake()
    {
        Net_MessageInterpreter.OnPlayerLogin += RequestWorldData;
    }

    private void RequestWorldData()
    {
        Wrapper wrapper = new Wrapper();
        RequestLogin request = new RequestLogin();
        request.PlayerId = NetworkCredits.PlayerId;

        wrapper.Type = Wrapper.Types.MessageType.Worlddata;
        wrapper.Payload = request.ToByteString();
        Net_ConnectionHandler.Instance.SendSpesteraMessage_TCP(wrapper,false);
    }
}
