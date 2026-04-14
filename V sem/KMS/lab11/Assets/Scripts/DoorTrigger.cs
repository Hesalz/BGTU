using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;
    public float openAngle = 90f;
    public float openSpeed = 50f;

    public GameObject flyingCube;
    public float cubeRotationSpeed = 100f;
    public float cubeFlySpeed = 2f;

    private bool playerInTrigger = false;
    private bool robotInTrigger = false;
    private Quaternion leftDoorClosedRotation;
    private Quaternion rightDoorClosedRotation;
    private Quaternion leftDoorOpenRotation;
    private Quaternion rightDoorOpenRotation;

    private bool cubeIsFlying = false;
    private Vector3 cubeStartPosition;
    private float cubeTimeCounter = 0f;

    private void Start()
    {
        if (leftDoor != null)
        {
            leftDoorClosedRotation = leftDoor.transform.rotation;
            leftDoorOpenRotation = leftDoorClosedRotation * Quaternion.Euler(0f, -openAngle, 0f);
        }

        if (rightDoor != null)
        {
            rightDoorClosedRotation = rightDoor.transform.rotation;
            rightDoorOpenRotation = rightDoorClosedRotation * Quaternion.Euler(0f, openAngle, 0f);
        }

        if (flyingCube != null)
        {
            cubeStartPosition = flyingCube.transform.position;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.name == "player")
        {
            playerInTrigger = true;
        }
        else if (col.name == "robot")
        {
            robotInTrigger = true;
            cubeIsFlying = true;
            cubeTimeCounter = 0f;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.name == "player")
        {
            playerInTrigger = false;
        }
        else if (col.name == "robot")
        {
            robotInTrigger = false;
            cubeIsFlying = false;

            if (flyingCube != null)
            {
                flyingCube.transform.position = cubeStartPosition;
                flyingCube.transform.rotation = Quaternion.identity;
            }
        }
    }

    private void Update()
    {
        ManageDoors();
        ManageFlyingCube();
    }

    private void ManageDoors()
    {
        if (leftDoor != null && rightDoor != null)
        {
            if (playerInTrigger || robotInTrigger)
            {
                leftDoor.transform.rotation = Quaternion.RotateTowards(leftDoor.transform.rotation,leftDoorOpenRotation, openSpeed * Time.deltaTime);
                rightDoor.transform.rotation = Quaternion.RotateTowards(rightDoor.transform.rotation, rightDoorOpenRotation, openSpeed * Time.deltaTime);
            }
            else
            {
                leftDoor.transform.rotation = Quaternion.RotateTowards(leftDoor.transform.rotation, leftDoorClosedRotation, openSpeed * Time.deltaTime);
                rightDoor.transform.rotation = Quaternion.RotateTowards(rightDoor.transform.rotation, rightDoorClosedRotation, openSpeed * Time.deltaTime);
            }
        }
    }

    private void ManageFlyingCube()
    {
        if (flyingCube != null && cubeIsFlying)
        {
            flyingCube.transform.Rotate(0f, cubeRotationSpeed * Time.deltaTime, 0f);

            cubeTimeCounter += Time.deltaTime * cubeFlySpeed;

            float zMovement = Mathf.Sin(cubeTimeCounter) * 5f;

            flyingCube.transform.position = new Vector3(
                cubeStartPosition.x,
                cubeStartPosition.y,
                cubeStartPosition.z + zMovement
            );
        }
    }
}