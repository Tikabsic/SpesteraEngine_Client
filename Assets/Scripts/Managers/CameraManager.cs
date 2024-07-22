using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform player;
    public string mouseButton = "Fire2";
    public float rotationSpeed = 10f;
    public float minZoomPercent = 0.4f;
    public Vector3 offset;
    public float zoomSpeed = 2f;
    private float initialY;
    private float initialZ;
    private float minY;
    private float minZ;


    private bool isRotating = false;
    private CinemachineTransposer transposer;
    public float initialCameraRotationX;

    private Camera mainCamera;

    void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        transposer.m_FollowOffset = offset;

        initialY = transposer.m_FollowOffset.y;
        initialZ = transposer.m_FollowOffset.z;
        minY = initialY * minZoomPercent;
        minZ = initialZ * minZoomPercent;

        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleRotation();
        HandleZoom();
        MaintainInitialRotation();


        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane));
        Vector3 topLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

    }

    void HandleRotation()
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

            transposer.m_FollowOffset = Quaternion.AngleAxis(horizontal, Vector3.up) * transposer.m_FollowOffset;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            Vector3 newOffset = transposer.m_FollowOffset;

            float newZoomLevel = Mathf.Clamp01((newOffset.y - scroll * zoomSpeed - minY) / (initialY - minY));

            newOffset.y = Mathf.Lerp(minY, initialY, newZoomLevel);
            newOffset.z = Mathf.Lerp(minZ, initialZ, newZoomLevel);

            transposer.m_FollowOffset = newOffset;
        }
    }

    void MaintainInitialRotation()
    {
        Vector3 currentRotation = virtualCamera.transform.eulerAngles;
        virtualCamera.transform.rotation = Quaternion.Euler(initialCameraRotationX, currentRotation.y, currentRotation.z);
    }
}

