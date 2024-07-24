using TMPro;
using UnityEngine;

public class Player_Dummy : MonoBehaviour
{
    public uint _pDummyId;
    public float rotationSpeed = 5f;
    public float movementspeed = 5f;
    private Quaternion targetRotation = Quaternion.identity;
    private Vector3 _targetPosition;

    private CharacterController _characterController;


    //Base motion animations handling
    Animator _animator;
    [SerializeField] private bool _isRunning;
    [SerializeField] private bool _isPlaying;
    [SerializeField] private bool _isIdle;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        targetRotation = transform.rotation;

        _isPlaying = false;
        _isIdle = true;

    }

    public void SetDummyTransform()
    {
        if (_targetPosition != Vector3.zero)
        {
            var direction = _targetPosition - transform.position;
            var moveDistance = movementspeed * Time.fixedDeltaTime;

            if (direction.magnitude <= moveDistance)
            {
                _characterController.Move(direction);
                _targetPosition = Vector3.zero;
            }
            else
            {
                var moveDirection = direction.normalized * moveDistance;

                Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
                if (lookDirection != Vector3.zero)
                {
                    Quaternion newRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.fixedDeltaTime);
                }

                _characterController.Move(moveDirection);
            }
            _isRunning = direction.magnitude > 0.01f;
        }
        else
        {
            if (_isRunning)
            {
                _isRunning = false;
            }
        }
    }

    public void SetTargetPosition(PlayerPosition transform)
    {
        Vector3 targetTransform = new Vector3(transform.PositionX, transform.PositionY, transform.PositionZ);
        _targetPosition = targetTransform;
    }

    private void FixedUpdate()
    {
        SetDummyTransform();
        UpdateMotionAnimations();
    }

    private void UpdateMotionAnimations()
    {
        if (_isRunning)
        {
            if (!_isPlaying)
            {
                _animator.SetBool("_isRunning", true);
                _isPlaying = true;
                _isIdle = false;        
            }
}

        if (!_isRunning)
        {
            if (!_isIdle)
            {
                _animator.SetBool("_isRunning", false);
                _isIdle = true;
                _isPlaying = false;
            }
        }

    }
}