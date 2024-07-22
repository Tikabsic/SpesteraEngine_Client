using TMPro;
using UnityEngine;

public class Player_Dummy : MonoBehaviour
{
    public ushort _pDummyId;
    public float rotationSpeed = 5f;
    public float movementspeed = 5f;
    private Quaternion targetRotation = Quaternion.identity;
    private Vector3 _targetPosition;

    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        targetRotation = transform.rotation;
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
    }
}