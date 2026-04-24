using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Движение")]
    public float maxSpeed = 15f;
    public float acceleration = 8f;
    public float brakingForce = 10f;
    public float reverseSpeed = 7f;
    public float turnSensitivity = 1.2f;

    [Header("Колёса")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public float maxSteeringAngle = 35f;

    [Header("Вращение колёс")]
    public float wheelRotationSpeed = 500f;

    private Rigidbody rb;
    private bool isPlayerDriving = false;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    private float currentTurnInput = 0f;
    private float currentSteeringAngle = 0f;
    private float wheelRotation = 0f;

    private Quaternion originalLeftWheelRotation;
    private Quaternion originalRightWheelRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.centerOfMass = new Vector3(0, -0.8f, 0);
        }

        if (frontLeftWheel != null)
            originalLeftWheelRotation = frontLeftWheel.localRotation;
        if (frontRightWheel != null)
            originalRightWheelRotation = frontRightWheel.localRotation;

        DisableWheelAnimators();

        SetDrivingState(false);
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

        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float speedFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeed);
            float turnMultiplier = Mathf.Lerp(0.7f, 1f, 1 - speedFactor);
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
    }

    void ApplyWheelTransform(float rotationAngle, float steeringAngle)
    {
        Quaternion steeringRot = Quaternion.Euler(0, 0, steeringAngle);
        Quaternion wheelRot = Quaternion.Euler(rotationAngle, 0, 0);

        if (frontLeftWheel != null)
        {
            frontLeftWheel.localRotation = originalLeftWheelRotation * steeringRot * wheelRot;
        }

        if (frontRightWheel != null)
        {
            frontRightWheel.localRotation = originalRightWheelRotation * steeringRot * wheelRot;
        }
    }

    public void SetDrivingState(bool isDriving)
    {
        isPlayerDriving = isDriving;

        if (!isDriving)
        {
            currentSpeed = 0f;
            wheelRotation = 0f;
            currentSteeringAngle = 0f;

            if (frontLeftWheel != null)
                frontLeftWheel.localRotation = originalLeftWheelRotation;
            if (frontRightWheel != null)
                frontRightWheel.localRotation = originalRightWheelRotation;
        }

        if (rb != null)
        {
            rb.isKinematic = !isDriving;
            if (!isDriving) rb.linearVelocity = Vector3.zero;
        }
    }
}