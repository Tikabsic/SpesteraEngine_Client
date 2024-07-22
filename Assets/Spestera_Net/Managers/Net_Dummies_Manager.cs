using Google.Protobuf.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_Dummies_Manager : MonoBehaviour
{

    public List<Player_Dummy> _playerDummies = new List<Player_Dummy>();
    public List<MOB_Dummy> _mobDummies = new List<MOB_Dummy>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Net_HeartbeatHandler.Instance.OnPositionUpdate_event += UpdateDummiesTransform;
    }

    private void UpdateDummiesTransform(Heartbeat hb)
    {
        NetworkManager._syncContext.Post(_ =>
        {
            UpdatePlayerDummiesTransform(hb.Players);
        }, null);

    }

    private void UpdatePlayerDummiesTransform(RepeatedField<PlayerPosition> players)
    {
        NetworkManager._syncContext.Post(_ =>
        {
            foreach (var playerData in players)
            {
                var pdummy = _playerDummies.Find(x => x._pDummyId == playerData.PlayerId);
                if (pdummy != null)
                {
                    pdummy.SetTargetPosition(playerData);
                }
            }
        }, null);

    }

    private void UpdateMonsterDummiesTransform(RepeatedField<MonsterPosition> monsters)
    {

    }

    private void RemoveLoggedoutPlayer(uint playerId)
    {
        Debug.Log($"Client with {playerId} id has logged out!");
    }
}
