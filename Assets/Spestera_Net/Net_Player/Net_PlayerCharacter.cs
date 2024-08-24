using UnityEngine;

public class Net_PlayerCharacter : MonoBehaviour
{
    private Net_PlayerController net_controller;
    private void Awake()
    {
        net_controller = GetComponent<Net_PlayerController>();
        Net_MessageInterpreterZS.OnPlayerDataRecived += ProcessInitializeData;
    }

    private void ProcessInitializeData(PlayerInitialData data)
    {
            net_controller.SetPlayerSpeed((float)data.PlayerMovementspeed);
            Vector3 initialPosition = new Vector3((float)data.PositionX, (float)data.PositionY, (float)data.PositionZ);
            net_controller.TeleportPlayerToSpecificLocation(initialPosition);
            net_controller.SetInitialCorrectPosition(initialPosition);
    }
}
