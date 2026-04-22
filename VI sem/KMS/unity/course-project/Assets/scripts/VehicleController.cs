using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 10f;
    public float turnSpeed = 50f;
    public float smoothTime = 0.1f;

    [Header("Колёса")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public float maxWheelAngle = 35f;

    [Header("Вращение колёс")]
    public bool rotateWheels = true;
    public float wheelRotationSpeed = 100f;

    private float currentSpeed = 0f;
    private float currentTurn = 0f;
    private float velocityRef = 0f;
    private float turnVelocityRef = 0f;
    private float wheelRotation = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.centerOfMass = new Vector3(0, -0.5f, 0);
        }
    }

    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        currentSpeed = Mathf.SmoothDamp(currentSpeed, vertical * moveSpeed, ref velocityRef, smoothTime);
        currentTurn = Mathf.SmoothDamp(currentTurn, horizontal * turnSpeed, ref turnVelocityRef, smoothTime);

        Vector3 move = transform.forward * currentSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        transform.Rotate(Vector3.up, currentTurn * Time.deltaTime);

        float wheelAngle = horizontal * maxWheelAngle;

        if (frontLeftWheel != null)
            frontLeftWheel.localRotation = Quaternion.Euler(0, wheelAngle, 0);

        if (frontRightWheel != null)
            frontRightWheel.localRotation = Quaternion.Euler(0, wheelAngle, 0);

        if (rotateWheels && currentSpeed != 0)
        {
            float rotationDelta = currentSpeed * wheelRotationSpeed * Time.deltaTime;
            wheelRotation += rotationDelta;

            if (frontLeftWheel != null)
                frontLeftWheel.Rotate(rotationDelta, 0, 0, Space.Self);

            if (frontRightWheel != null)
                frontRightWheel.Rotate(rotationDelta, 0, 0, Space.Self);
        }
    }
}