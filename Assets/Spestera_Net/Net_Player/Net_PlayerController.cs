using Google.Protobuf;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Net_PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed;
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float rotationSpeed = 10f;


    //Transform handling properties
    private CharacterController characterController;
    //private Vector3 movementDirection;
    private Vector3 _lastPosition;
    private Vector3 _lastCorrectPosition;
    private Vector3 _initialCorrectPosition;

    private Vector3 movementDirection;
    private float epsilon = 0.0001f;
    private bool _isGathering = true;

    //Bytes calculation properties
    private int bytesSent;
    private float totalBytesSent;
    private int byteCounter;
    private float lastSentTime;
    [SerializeField]private float averageBytesPerSecond;


    //Base motion animation properites
    public bool _isRunning;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        _lastPosition = transform.position;
    }

    private void Start()
    {
        Net_HeartbeatHandler.Instance.OnPositionUpdate_event += ValidatePlayerPosition;
        StartCoroutine(SendPlayerTransformCoroutine());
    }

    private void Update()
    {
        if (!_isGathering)
        {
            return;
        }
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        Vector3 camForwardXZ = Vector3.ProjectOnPlane(camForward, Vector3.up).normalized;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirectionXZ = camForwardXZ * vertical + camRight * horizontal;
        moveDirectionXZ.Normalize();

        Vector3 moveDirection = new Vector3(moveDirectionXZ.x, 0f, moveDirectionXZ.z);
        moveDirection.Normalize();

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        Vector3 gravityMove = -transform.up * gravity;

        Vector3 finalMove = (moveDirection + gravityMove) * speed * Time.deltaTime;

        //Vector3 roundedMove = new Vector3(
        //    Mathf.Round(finalMove.x * 100) / 100,
        //    finalMove.y,
        //    Mathf.Round(finalMove.z * 100) / 100);

        _lastPosition = transform.position;
        characterController.Move(finalMove);

        movementDirection += (transform.position - _lastPosition) / speed;

        _isRunning = characterController.velocity.magnitude > 0f;

        UpdateAverageBytesPerSecond();
    }

    [SerializeField] private int _roundMultiplier;
    public void RoundVectorToByteValues(ref Vector3 movementVector, int decimalPoints)
    {

        movementVector.x = RoundToDecimalPlaces(ref movementVector.x, decimalPoints) * _roundMultiplier;
        movementVector.z = RoundToDecimalPlaces(ref movementVector.z, decimalPoints) * _roundMultiplier;
    }

    public float RoundToDecimalPlaces(ref float number, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(number * multiplier) / multiplier;
    }

    public float SendPositionFrequency;

    private IEnumerator SendPlayerTransformCoroutine()
    {
        while (true)
        {
            if (movementDirection != Vector3.zero)
            {
                _isGathering = false;
                RoundVectorToByteValues(ref movementDirection, 4);
                SendPlayerTransform();
                movementDirection = Vector3.zero;
                _isGathering = true;
            }

            yield return new WaitForSecondsRealtime(SendPositionFrequency);
        }
    }

    public void TeleportPlayerToSpecificLocation(Vector3 location)
    {
        characterController.transform.position = location;
    }

    private void SendPlayerTransform()
    {
        if (Mathf.Abs(movementDirection.y) < epsilon || Mathf.Abs(movementDirection.y) < -epsilon)
        {
            movementDirection.y = 0;
        }

        PlayerPosition playerPosition = new PlayerPosition
        {
            PositionX = (sbyte)movementDirection.x,
            PositionY = movementDirection.y != 0 ? movementDirection.y : 0,
            PositionZ = (sbyte)movementDirection.z
        };

        Wrapper wrapper = new Wrapper
        {
            Type = Wrapper.Types.MessageType.Playerposition,
            Payload = playerPosition.ToByteString()
        };

        int messageLength = wrapper.ToByteArray().Length;
        CalculateBytes(messageLength);

        Vector3 testpos = transform.position;
        Debug.Log($"should be at : {testpos + ((movementDirection / _roundMultiplier))} // x y z {(float)playerPosition.PositionX / 1000}, {(float)playerPosition.PositionY / 1000}, {(float)playerPosition.PositionZ / 1000}");

        Net_ConnectionHandler.Instance.SendSpesteraMessage_TCP(wrapper, false);
        movementDirection = Vector3.zero;
    }

    private void CalculateBytes(int bytesLength)
    {
        bytesSent += bytesLength;
        byteCounter++;
    }

    private void UpdateAverageBytesPerSecond()
    {
        if (Time.time - lastSentTime >= 1f)
        {
            averageBytesPerSecond = bytesSent / (Time.time - lastSentTime);
            lastSentTime = Time.time;
            bytesSent = 0;
        }
    }

    public void SetPlayerSpeed(float ms)
    {
        speed = ms;
    }

    public float GetAverageBytesPerSecond()
    {
        return averageBytesPerSecond;
    }

    private Vector3 lastReceivedPosition;
    private float interpolationFactor = 0.2f;
    public Vector3 interpolatedPosition;
    public int badPositionConuter;
    [SerializeField] private int maximumLostPositions;

    private void ValidatePlayerPosition(Heartbeat hb)
    {
        var playerTransform = hb.Players.FirstOrDefault(x => x.PlayerId == NetworkCredits.PlayerId);

        if (playerTransform != null)
        {

            Vector3 correctTransform = new Vector3(playerTransform.PositionX, playerTransform.PositionY, playerTransform.PositionZ);
            lastReceivedPosition = correctTransform;

                if (characterController.transform.position == correctTransform)
                {
                    return;
                }

                Vector3 currentPosition = this.transform.position;

                interpolatedPosition = Vector3.Lerp(currentPosition, lastReceivedPosition, interpolationFactor);


                bool isApproximatelyEqual = Mathf.Abs(interpolatedPosition.x - correctTransform.x) <= 0.2f &&
                                            Mathf.Abs(interpolatedPosition.y - correctTransform.y) <= 0.2f &&
                                            Mathf.Abs(interpolatedPosition.z - correctTransform.z) <= 0.2f;

                if (isApproximatelyEqual)
                {
                    _lastCorrectPosition = correctTransform;
                    badPositionConuter = 0;
                }
                else
                {
                    badPositionConuter++;
                    if (badPositionConuter >= maximumLostPositions)
                    {
                        Debug.Log("Cheater");
                        characterController.transform.position = correctTransform;
                    }
                }
        }
    }

    public void SetInitialCorrectPosition(Vector3 correctPosition)
    {
        _initialCorrectPosition = correctPosition;
    }
}