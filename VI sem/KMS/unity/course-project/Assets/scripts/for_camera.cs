using UnityEngine;

public class for_camera : MonoBehaviour
{
    [SerializeField] private Transform followTarget;

    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float zoomSpeed = 100f;

    [SerializeField] private float nearLimit = 20f;
    [SerializeField] private float farLimit = 80f;

    [SerializeField] private float downMin = -20f;
    [SerializeField] private float upMax = 80f;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float moveLimitX = 20f;
    [SerializeField] private float moveLimitZ = 20f;

    private float horizontalAngle = 0f;
    private float verticalAngle = 20f;
    private float currentZoom = 100f;

    private Vector3 cameraOffset = Vector3.zero;

    private void Awake()
    {
        Vector3 startRotation = transform.eulerAngles;
        horizontalAngle = startRotation.y;
        verticalAngle = startRotation.x;

        SetCursorState(true);
    }

    private void Update()
    {
        HandleCursorUnlock();
        HandleCameraRotation();
        HandleZoom();
        HandleCameraMovement();
    }

    private void LateUpdate()
    {
        if (followTarget == null) return;

        ApplyCameraTransform();
    }

    private void SetCursorState(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void HandleCursorUnlock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorState(false);
        }
    }

    private void HandleCameraRotation()
    {
        if (!Input.GetMouseButton(1)) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        horizontalAngle += mouseX * rotationSpeed;
        verticalAngle -= mouseY * rotationSpeed;

        verticalAngle = Mathf.Clamp(verticalAngle, downMin, upMax);
    }

    private void HandleZoom()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        currentZoom += -scrollDelta * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, nearLimit, farLimit);
    }

    private void HandleCameraMovement()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalMove, 0f, verticalMove);
        Vector3 localMove = transform.right * moveDirection.x + transform.forward * moveDirection.z;
        localMove.y = 0f;

        cameraOffset += localMove * moveSpeed * Time.deltaTime;

        cameraOffset.x = Mathf.Clamp(cameraOffset.x, -moveLimitX, moveLimitX);
        cameraOffset.z = Mathf.Clamp(cameraOffset.z, -moveLimitZ, moveLimitZ);
    }

    private void ApplyCameraTransform()
    {
        Quaternion cameraRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        Vector3 offset = cameraRotation * new Vector3(0f, 0f, -currentZoom);
        Vector3 targetPosition = followTarget.position + offset + cameraOffset;

        transform.position = targetPosition;
        transform.LookAt(followTarget);
    }
}