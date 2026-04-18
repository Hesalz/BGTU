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

    private bool isFreeControlEnabled = true;
    private bool isAutoMoving = false;
    private Vector3 autoTargetPosition;
    private Quaternion autoTargetRotation;
    private float autoMoveProgress = 0f;
    private float autoMoveSpeed = 3f;

    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private Vector3 savedCameraOffset;
    private float savedHorizontalAngle;
    private float savedVerticalAngle;
    private float savedZoom;

    private bool isUIMode = true;
    public GameObject[] uiPanelsToHide;

    private bool isManualPosition = false;

    private bool isVehicleMode = false;

    private float originalRotationSpeed;
    private float originalZoomSpeed;
    private float originalMoveSpeed;
    private float originalNearLimit;
    private float originalFarLimit;

    private void Awake()
    {
        originalRotationSpeed = rotationSpeed;
        originalZoomSpeed = zoomSpeed;
        originalMoveSpeed = moveSpeed;
        originalNearLimit = nearLimit;
        originalFarLimit = farLimit;

        Vector3 startRotation = transform.eulerAngles;
        horizontalAngle = startRotation.y;
        verticalAngle = startRotation.x;

        SetUIMode(true);
        SaveCurrentState();
    }

    private void Update()
    {
        if (isAutoMoving)
        {
            HandleAutoMove();
            return;
        }

        if (isFreeControlEnabled && !isUIMode && isVehicleMode)
        {
            HandleCameraRotation();
            HandleZoom();
            HandleCameraMovement();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isUIMode)
            {
                SetUIMode(false);
            }
            else
            {
                SetUIMode(true);
            }
        }
    }

    private void LateUpdate()
    {
        if (followTarget == null) return;
        if (isAutoMoving) return;
        if (isManualPosition) return;

        if (!isUIMode && isFreeControlEnabled && isVehicleMode)
        {
            ApplyCameraTransform();
        }
    }

    public void SetUIMode(bool uiMode)
    {
        isUIMode = uiMode;

        if (uiMode)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isFreeControlEnabled = false;
            isManualPosition = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isFreeControlEnabled = true;
            isManualPosition = false;
        }
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
        SaveCurrentState();
    }

    private void HandleAutoMove()
    {
        autoMoveProgress += Time.deltaTime * autoMoveSpeed;

        if (autoMoveProgress >= 1f)
        {
            transform.position = autoTargetPosition;
            transform.rotation = autoTargetRotation;
            isAutoMoving = false;
            isManualPosition = true;
        }
        else
        {
            float t = Mathf.SmoothStep(0f, 1f, autoMoveProgress);
            transform.position = Vector3.Lerp(transform.position, autoTargetPosition, t);
            transform.rotation = Quaternion.Slerp(transform.rotation, autoTargetRotation, t);
        }
    }

    public void SaveCurrentState()
    {
        savedPosition = transform.position;
        savedRotation = transform.rotation;
        savedCameraOffset = cameraOffset;
        savedHorizontalAngle = horizontalAngle;
        savedVerticalAngle = verticalAngle;
        savedZoom = currentZoom;
    }

    public void RestoreSavedState()
    {
        isAutoMoving = false;
        isManualPosition = false;

        transform.position = savedPosition;
        transform.rotation = savedRotation;

        cameraOffset = savedCameraOffset;
        horizontalAngle = savedHorizontalAngle;
        verticalAngle = savedVerticalAngle;
        currentZoom = savedZoom;
    }

    public void MoveToElement(Vector3 targetPosition, Quaternion targetRotation)
    {
        SaveCurrentState();

        isFreeControlEnabled = false;
        isManualPosition = false;

        autoTargetPosition = targetPosition;
        autoTargetRotation = targetRotation;
        autoMoveProgress = 0f;
        isAutoMoving = true;
    }

    public void StartPracticeMode()
    {
        HideAllUIPanels(true);
        SetUIMode(false);
        isVehicleMode = false;
        isFreeControlEnabled = false;
    }

    public void ExitPracticeMode()
    {
        HideAllUIPanels(false);
        SetUIMode(true);
        isVehicleMode = false;
    }

    private void HideAllUIPanels(bool hide)
    {
        foreach (GameObject panel in uiPanelsToHide)
        {
            if (panel != null)
                panel.SetActive(!hide);
        }
    }

    private void HandleCameraRotation()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            horizontalAngle += mouseX * rotationSpeed;
            verticalAngle -= mouseY * rotationSpeed;
            verticalAngle = Mathf.Clamp(verticalAngle, downMin, upMax);
        }
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
        if (followTarget == null) return;

        Quaternion cameraRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        Vector3 offset = cameraRotation * new Vector3(0f, 0f, -currentZoom);
        Vector3 targetPosition = followTarget.position + offset + cameraOffset;

        transform.position = targetPosition;
        transform.LookAt(followTarget);
    }

    public void SetVehicleMode(bool isVehicle)
    {
        isVehicleMode = isVehicle;

        if (isVehicleMode)
        {
            rotationSpeed = originalRotationSpeed;
            zoomSpeed = originalZoomSpeed;
            moveSpeed = originalMoveSpeed;
            nearLimit = originalNearLimit;
            farLimit = originalFarLimit;

            isFreeControlEnabled = true;
            isManualPosition = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            horizontalAngle = 0f;
            verticalAngle = 20f;
            cameraOffset = Vector3.zero;
        }
        else
        {
            rotationSpeed = 0f;
            moveSpeed = 0f;
            zoomSpeed = 0f;
            isFreeControlEnabled = false;
        }
    }
}