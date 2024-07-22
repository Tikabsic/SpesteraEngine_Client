using Google.Protobuf;
using System;
using UnityEngine;

public class Net_HeartbeatHandler : Singleton<Net_HeartbeatHandler>
{
    public event Action OnUpdatePing_event;
    public event Action<Heartbeat> OnPositionUpdate_event;

    private int _lastHeartbeatTimestamp = 0;

    public void HandleMessage(Heartbeat message)
    {
        CalculatePing();
        HandleHeartbeatPayload(message);
    }

    private void CalculatePing()
    {
        if (OnUpdatePing_event != null)
        {
            OnUpdatePing_event.Invoke();
        }

    }

    private void HandleHeartbeatPayload(Heartbeat hb)
    {
        OnPositionUpdate_event?.Invoke(hb);
    }
}
