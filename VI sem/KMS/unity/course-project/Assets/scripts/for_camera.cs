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
    //[SerializeField] private float moveLimitX = 20f;
    //[SerializeField] private float moveLimitZ = 20f;

    [Header("Режимы камеры")]
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0, 2, -5);
    [SerializeField] private Vector3 firstPersonOffset = new Vector3(-1.3f, 4f, 2f);

    private float horizontalAngle = 0f;
    private float verticalAngle = 20f;
    private float currentZoom = 50f;

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
    private int currentCameraMode = 2;

    private float firstPersonHorizontal = 0f;
    private float firstPersonVertical = 0f;
    private float thirdPersonHorizontal = 0f;
    private float thirdPersonVertical = 20f;
    private float thirdPersonZoom = 50f;

    private float originalRotationSpeed;
    private float originalZoomSpeed;
    private float originalMoveSpeed;
    private float originalNearLimit;
    private float originalFarLimit;

    private Transform originalCameraParent;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    private void Awake()
    {
        originalRotationSpeed = rotationSpeed;
        originalZoomSpeed = zoomSpeed;
        originalMoveSpeed = moveSpeed;
        originalNearLimit = nearLimit;
        originalFarLimit = farLimit;

        originalCameraParent = transform.parent;
        originalCameraPosition = transform.localPosition;
        originalCameraRotation = transform.localRotation;

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

        if (isVehicleMode && !isUIMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetCameraMode(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetCameraMode(2);
            }
        }

        if (isVehicleMode && !isUIMode && isFreeControlEnabled)
        {
            HandleCameraRotation();

            if (currentCameraMode == 2)
            {
                HandleZoom();
            }
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

        if (isVehicleMode && !isUIMode)
        {
            ApplyCameraTransform();
        }
    }

    public void SetCameraMode(int mode)
    {
        if (mode == currentCameraMode) return;

        if (currentCameraMode == 1)
        {
            firstPersonHorizontal = horizontalAngle;
            firstPersonVertical = verticalAngle;
        }
        else if (currentCameraMode == 2)
        {
            thirdPersonHorizontal = horizontalAngle;
            thirdPersonVertical = verticalAngle;
            thirdPersonZoom = currentZoom;
        }

        currentCameraMode = mode;

        if (currentCameraMode == 1)
        {
            horizontalAngle = firstPersonHorizontal;
            verticalAngle = firstPersonVertical;
            currentZoom = nearLimit + 10f;

            if (followTarget != null)
            {
                transform.SetParent(followTarget);
                transform.localPosition = firstPersonOffset;
                transform.localRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
            }
        }
        else
        {
            horizontalAngle = thirdPersonHorizontal;
            verticalAngle = thirdPersonVertical;
            currentZoom = thirdPersonZoom;

            transform.SetParent(originalCameraParent);
            transform.position = savedPosition;
            transform.rotation = savedRotation;
            cameraOffset = Vector3.zero;
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
        if (!Input.GetMouseButton(1)) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (currentCameraMode == 1)
        {
            horizontalAngle += mouseX * rotationSpeed;
            verticalAngle -= mouseY * rotationSpeed;
            verticalAngle = Mathf.Clamp(verticalAngle, -80f, 80f);

            transform.localRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        }
        else
        {
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

    private void ApplyCameraTransform()
    {
        if (followTarget == null) return;

        if (currentCameraMode == 1) return;

        Quaternion cameraRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        Vector3 offset = cameraRotation * new Vector3(0f, 0f, -currentZoom);
        Vector3 targetPosition = followTarget.position + offset + cameraOffset + thirdPersonOffset;

        transform.position = targetPosition;
        transform.LookAt(followTarget.position + Vector3.up * 1.5f);
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

            SetCameraMode(2);
            cameraOffset = Vector3.zero;

            horizontalAngle = 0f;
            verticalAngle = 20f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            transform.SetParent(originalCameraParent);
            transform.localPosition = originalCameraPosition;
            transform.localRotation = originalCameraRotation;

            rotationSpeed = 0f;
            moveSpeed = 0f;
            zoomSpeed = 0f;
            isFreeControlEnabled = false;
        }
    }

    public int GetCurrentCameraMode()
    {
        return currentCameraMode;
    }
}