using Google.Protobuf;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class Net_PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed;
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float rotationSpeed = 10f;


    //Transform handling properties
    private CharacterController characterController;
    private Vector3 movementDirection;
    private Vector3 _lastPosition;
    private Vector3 _lastCorrectPosition;
    private Vector3 _initialCorrectPosition;


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

    private void FixedUpdate()
    {
        if (characterController.isGrounded)
        {
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

            movementDirection += moveDirection;

            Vector3 finalMove = moveDirection * speed;

            _lastPosition = transform.position;
            characterController.Move(finalMove * Time.fixedDeltaTime);
            _isRunning = characterController.velocity.magnitude > 0.01f;
        }

        Vector3 gravityMove = -transform.up * gravity;
        characterController.Move(gravityMove * Time.fixedDeltaTime);
    }

    public float SendPositionFrequency;

    private IEnumerator SendPlayerTransformCoroutine()
    {
        while (true)
        {
            if (movementDirection != Vector3.zero)
            {
                SendPlayerTransform();
                movementDirection = Vector3.zero;
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
        PlayerPosition playerPosition = new PlayerPosition
        {
            PositionX = movementDirection.x != 0 ? movementDirection.x : 0,
            PositionY = movementDirection.y != 0 ? movementDirection.y : 0,
            PositionZ = movementDirection.z != 0 ? movementDirection.z : 0,
        };

        Wrapper wrapper = new Wrapper
        {
            Type = Wrapper.Types.MessageType.Playerposition,
            Payload = playerPosition.ToByteString()
        };

        int messageLength = wrapper.ToByteArray().Length;
        CalculateBytes(messageLength);

        Net_ConnectionHandler.Instance.SendSpesteraMessage_TCP(wrapper, false);
        movementDirection = Vector3.zero;
    }

    private void CalculateBytes(int bytesLength)
    {
        bytesSent += bytesLength;
        byteCounter++;
    }

    private void Update()
    {
        UpdateAverageBytesPerSecond();
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
    private float interpolationFactor = 0.1f;
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