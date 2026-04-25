using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Движение")]
    public float maxSpeed = 40f;
    public float acceleration = 10f;
    public float brakingForce = 10f;
    public float reverseSpeed = 10f;
    public float turnSensitivity = 1.2f;

    [Header("Колёса")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public float maxSteeringAngle = 35f;

    [Header("Руль (опционально)")]
    public Transform steeringWheel;
    public float maxSteeringWheelAngle = 180f;
    public bool invertSteeringWheel = false;
    public SteeringWheelAxis steeringAxis = SteeringWheelAxis.Z;

    public enum SteeringWheelAxis { X, Y, Z }

    [Header("Вращение колёс")]
    public float wheelRotationSpeed = 500f;

    [Header("Мигалка")]
    public Flasher vehicleFlasher; 
    public KeyCode flashKey = KeyCode.F;

    [Header("Настройки CharacterController")]
    public float controllerHeight = 0.5f;
    public float controllerRadius = 3.5f;
    public Vector3 controllerCenter = new Vector3(0, 0.5f, 0);

    private CharacterController controller;
    private bool isPlayerDriving = false;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    private float currentTurnInput = 0f;
    private float currentSteeringAngle = 0f;
    private float wheelRotation = 0f;

    private Quaternion originalLeftWheelRotation;
    private Quaternion originalRightWheelRotation;
    private Quaternion originalSteeringWheelRotation;

    private float fixedGroundHeight = 0f;
    private bool hasFixedHeight = false;

    void Start()
    {
        SetupCharacterController();
        FindGroundHeight();

        if (frontLeftWheel != null)
            originalLeftWheelRotation = frontLeftWheel.localRotation;
        if (frontRightWheel != null)
            originalRightWheelRotation = frontRightWheel.localRotation;
        if (steeringWheel != null)
            originalSteeringWheelRotation = steeringWheel.localRotation;

        DisableWheelAnimators();
        SetDrivingState(false);
    }

    void SetupCharacterController()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }

        controller.height = controllerHeight;
        controller.radius = controllerRadius;
        controller.center = controllerCenter;
        controller.skinWidth = 0.01f;
        controller.minMoveDistance = 0.001f;
        controller.slopeLimit = 45f;
        controller.stepOffset = 0.3f;
        controller.detectCollisions = true;
    }

    void FindGroundHeight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, 10f))
        {
            fixedGroundHeight = hit.point.y + (controllerHeight / 2) - 0.01f;
            hasFixedHeight = true;

            Vector3 newPos = transform.position;
            newPos.y = fixedGroundHeight;
            transform.position = newPos;
        }
        else
        {
            fixedGroundHeight = transform.position.y;
        }
    }

    void FixedUpdate()
    {
        if (!isPlayerDriving) return;

        if (hasFixedHeight)
        {
            Vector3 pos = transform.position;
            pos.y = fixedGroundHeight;
            transform.position = pos;
        }
    }

    void Update()
    {
        if (!isPlayerDriving) return;

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (Mathf.Abs(vertical) < 0.1f)
        {
            targetSpeed = 0f;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, brakingForce * Time.deltaTime);
        }
        else
        {
            targetSpeed = vertical > 0 ? vertical * maxSpeed : vertical * reverseSpeed;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }

        Vector3 moveDirection = transform.forward * currentSpeed;

        moveDirection.y = 0;

        controller.Move(moveDirection * Time.deltaTime);

        if (hasFixedHeight && Mathf.Abs(transform.position.y - fixedGroundHeight) > 0.05f)
        {
            Vector3 pos = transform.position;
            pos.y = fixedGroundHeight;
            transform.position = pos;
        }

        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float speedFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeed);
            float turnMultiplier = Mathf.Lerp(0.5f, 1f, 1 - speedFactor);
            float turnAngle = horizontal * turnSensitivity * turnMultiplier * Mathf.Abs(currentSpeed) * Time.deltaTime;
            if (currentSpeed < 0) turnAngle = -turnAngle;
            transform.Rotate(Vector3.up, turnAngle);
        }

        currentTurnInput = Mathf.Lerp(currentTurnInput, horizontal, 10f * Time.deltaTime);
        currentSteeringAngle = currentTurnInput * maxSteeringAngle;

        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            wheelRotation += currentSpeed * wheelRotationSpeed * Time.deltaTime;
            ApplyWheelTransform(wheelRotation, currentSteeringAngle);
        }
        else
        {
            ApplyWheelTransform(0, currentSteeringAngle);
        }

        ApplySteeringWheelTransform(currentTurnInput);

        if (Input.GetKeyDown(flashKey))
        {
            if (vehicleFlasher != null)
            {
                vehicleFlasher.ToggleFlasher();
            }
        }
    }

    void DisableWheelAnimators()
    {
        if (frontLeftWheel != null)
        {
            Animator anim = frontLeftWheel.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;
        }
        if (frontRightWheel != null)
        {
            Animator anim = frontRightWheel.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;
        }
    }

    void ApplyWheelTransform(float rotationAngle, float steeringAngle)
    {
        Quaternion steeringRot = Quaternion.Euler(0, 0, steeringAngle);
        Quaternion wheelRot = Quaternion.Euler(rotationAngle, 0, 0);

        if (frontLeftWheel != null)
            frontLeftWheel.localRotation = originalLeftWheelRotation * steeringRot * wheelRot;
        if (frontRightWheel != null)
            frontRightWheel.localRotation = originalRightWheelRotation * steeringRot * wheelRot;
    }

    void ApplySteeringWheelTransform(float turnInput)
    {
        if (steeringWheel == null) return;

        float steeringWheelAngle = turnInput * maxSteeringWheelAngle;
        if (invertSteeringWheel) steeringWheelAngle = -steeringWheelAngle;

        Quaternion additionalRotation = Quaternion.identity;
        switch (steeringAxis)
        {
            case SteeringWheelAxis.X: additionalRotation = Quaternion.Euler(steeringWheelAngle, 0, 0); break;
            case SteeringWheelAxis.Y: additionalRotation = Quaternion.Euler(0, steeringWheelAngle, 0); break;
            case SteeringWheelAxis.Z: additionalRotation = Quaternion.Euler(0, 0, steeringWheelAngle); break;
        }

        steeringWheel.localRotation = originalSteeringWheelRotation * additionalRotation;
    }

    public void SetDrivingState(bool isDriving)
    {
        isPlayerDriving = isDriving;

        if (!isDriving)
        {
            currentSpeed = 0f;
            targetSpeed = 0f;
            wheelRotation = 0f;
            currentSteeringAngle = 0f;

            if (frontLeftWheel != null)
                frontLeftWheel.localRotation = originalLeftWheelRotation;
            if (frontRightWheel != null)
                frontRightWheel.localRotation = originalRightWheelRotation;
            if (steeringWheel != null)
                steeringWheel.localRotation = originalSteeringWheelRotation;
        }
        else
        {
            FindGroundHeight();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (controller != null)
        {
            Gizmos.color = Color.green;
            Vector3 center = transform.position + controller.center;
            Gizmos.DrawWireSphere(center, controller.radius);

            Vector3 top = center + Vector3.up * (controller.height / 2);
            Vector3 bottom = center - Vector3.up * (controller.height / 2);
            Gizmos.DrawLine(top, bottom);
        }

        if (hasFixedHeight)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(transform.position.x, fixedGroundHeight, transform.position.z), 0.2f);
        }
    }
}