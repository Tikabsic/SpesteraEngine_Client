using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_WorldManager : MonoBehaviour
{

    //private void Awake()
    //{
    //    Net_MessageInterpreterZS.OnPlayerInitialized += RequestWorldData;
    //}

    //private void RequestWorldData()
    //{
    //    ZSWrapper wrapper = new ZSWrapper();
    //    RequestWorldData request = new RequestWorldData();
    //    request.PlayerId = NetworkCredits.PlayerId;

    //    wrapper.Type = ZSWrapper.Types.MessageType.Requestworlddata;
    //    wrapper.Payload = request.ToByteString();
    //    Net_ConnectionHandler.Instance.SendSpesteraMessage_ZoneServer(wrapper,false);
    //}
}
