using Google.Protobuf.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Net_Dummies_Manager : MonoBehaviour
{
    [SerializeField] private Player_Dummy _playerDummyPrefab;

    public List<Player_Dummy> _playerDummies = new List<Player_Dummy>();
    public List<MOB_Dummy> _mobDummies = new List<MOB_Dummy>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Net_HeartbeatHandler.Instance.OnPositionUpdate_event += UpdateDummiesTransform;
        Net_MessageInterpreter.OnOtherPlayerLogout += RemoveLoggedoutPlayer;
        Net_MessageInterpreter.OnOtherPlayerLogin += InitializeNewPlayer;
    }

    private void UpdateDummiesTransform(Heartbeat hb)
    {

            UpdatePlayerDummiesTransform(hb.Players);

    }

    private void UpdatePlayerDummiesTransform(RepeatedField<PlayerPosition> players)
    {
            foreach (var playerData in players)
            {
                var pdummy = _playerDummies.Find(x => x._pDummyId == playerData.PlayerId);
                if (pdummy != null)
                {
                    pdummy.SetTargetPosition(playerData);
                }
            }

    }

    private void UpdateMonsterDummiesTransform(RepeatedField<MonsterPosition> monsters)
    {
        NetworkManager._syncContext.Post(_ =>
        {
            Debug.Log($"Mosnter dummy transform updated");
        }, null);
    }

    private void InitializeNewPlayer(PlayerInitialData data)
    {

            var newDummy = GameObject.Instantiate<Player_Dummy>(_playerDummyPrefab);
            newDummy._pDummyId = data.PlayerId;
            Vector3 initialPosition = new Vector3(data.PositionX, data.PositionY, data.PositionZ);
            newDummy.transform.position = initialPosition;
            newDummy.movementspeed = data.PlayerMovementspeed;


            _playerDummies.Add(newDummy);
            Debug.Log($"New player initialized with {newDummy._pDummyId} id!");
    }

    private void RemoveLoggedoutPlayer(uint playerId)
    {
        Debug.Log($"Dummies  manager should remove player with id {playerId}");

        if (_playerDummies.Any(x => x._pDummyId == playerId))
            {
                var loggedOutPlayer = _playerDummies.First(x => x._pDummyId == playerId);
                _playerDummies.Remove(loggedOutPlayer);
                Destroy(loggedOutPlayer.gameObject);
            }
    }
}
