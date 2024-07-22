using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pcontroller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed;
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float rotationSpeed = 10f;
    private CharacterController characterController;
    private Vector3 _lastPosition;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
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
            Vector3 finalMove = moveDirection * speed;
            characterController.Move(finalMove * Time.fixedDeltaTime);
        }

        Vector3 gravityMove = -transform.up * gravity;
        characterController.Move(gravityMove * Time.fixedDeltaTime);
    }
}
