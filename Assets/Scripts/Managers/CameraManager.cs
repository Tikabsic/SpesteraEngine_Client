using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public string mouseButton = "Fire2";
    public float rotationSpeed = 10f;
    public float minZoomPercent = 0.4f;
    public float zoomSpeed = 2f;
    public Vector3 offset;
    private float initialDistance;
    private float minDistance;

    private bool isRotating = false;
    private CinemachineTransposer transposer;

    private void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset = offset;

        initialDistance = offset.magnitude;
        minDistance = initialDistance * minZoomPercent;
    }

    private void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    private void HandleRotation()
    {
        if (Input.GetButtonDown(mouseButton))
        {
            isRotating = true;
        }
        if (Input.GetButtonUp(mouseButton))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;

            Quaternion rotation = Quaternion.Euler(0, horizontal, 0);
            transposer.m_FollowOffset = rotation * transposer.m_FollowOffset;
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            Vector3 offsetDirection = transposer.m_FollowOffset.normalized;
            float distance = transposer.m_FollowOffset.magnitude;

            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, initialDistance);

            transposer.m_FollowOffset = offsetDirection * distance;
        }
    }
}