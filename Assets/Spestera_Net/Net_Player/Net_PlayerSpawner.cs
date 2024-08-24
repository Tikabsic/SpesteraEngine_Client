using UnityEngine;

public class Net_PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _playerCharacter;
    void Start()
    {
        
    }


    private void SpawnPlayer()
    {
        GameObject.Instantiate(_playerCharacter);
    }
}
