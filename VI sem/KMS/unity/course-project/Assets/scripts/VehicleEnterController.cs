using UnityEngine;

public class VehicleEntryController : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject playerCharacter;
    public Camera playerCamera;
    public Camera vehicleCamera;
    public Transform vehicleCameraTarget;
    public Transform vehicleCameraPosition;
    public Transform exitFromVehiclePosition;
    public GameObject vehicleDoor;
    public GameObject enterTrigger;
    public float interactionDistance = 3f;

    [Header("Ссылки")]
    public for_camera vehicleCameraScript;
    public GameObject practiceButton;
    public OpenDoor doorScript;
    public VehicleController vehicleController;

    private bool isInVehicle = false;
    private bool isPracticeMode = false;
    private bool isDoorOpen = false;
    private Vector3 originalPlayerCameraPosition;
    private Quaternion originalPlayerCameraRotation;
    private Transform originalPlayerCameraParent;

    void Start()
    {
        if (playerCamera != null)
        {
            originalPlayerCameraParent = playerCamera.transform.parent;
            originalPlayerCameraPosition = playerCamera.transform.localPosition;
            originalPlayerCameraRotation = playerCamera.transform.localRotation;
        }

        if (vehicleCamera != null)
        {
            vehicleCamera.gameObject.SetActive(true);
        }

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);

        if (playerCharacter != null)
            playerCharacter.SetActive(false);

        if (vehicleCameraScript != null)
        {
            vehicleCameraScript.SetUIMode(true);
            if (vehicleCameraTarget != null)
                vehicleCameraScript.SetFollowTarget(vehicleCameraTarget);
            vehicleCameraScript.SetVehicleMode(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (isPracticeMode && !isInVehicle && Input.GetKeyDown(KeyCode.E))
        {
            if (IsLookingAtDoor())
            {
                if (doorScript != null)
                {
                    doorScript.ToggleDoor();
                    isDoorOpen = !isDoorOpen;
                }
            }
            else if (IsLookingAtEnterTrigger())
            {
                if (isDoorOpen)
                {
                    EnterVehicle();
                }
                else
                {
                }
            }
        }
        else if (isInVehicle && Input.GetKeyDown(KeyCode.E))
        {
            if (IsLookingAtDoor())
            {
                if (doorScript != null)
                {
                    doorScript.ToggleDoor();
                    isDoorOpen = !isDoorOpen;
                }
            }
            else
            {
                if (isDoorOpen)
                {
                    ExitVehicle();
                }
                else
                {
                }
            }
        }
    }

    bool IsLookingAtDoor()
    {
        Camera currentCamera = isInVehicle ? vehicleCamera : playerCamera;

        if (vehicleDoor == null || currentCamera == null) return false;

        Ray ray = new Ray(currentCamera.transform.position, currentCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.transform.gameObject == vehicleDoor || hit.transform.IsChildOf(vehicleDoor.transform))
            {
                return true;
            }
        }
        return false;
    }

    bool IsLookingAtEnterTrigger()
    {
        if (enterTrigger == null || playerCamera == null) return false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.transform.gameObject == enterTrigger || hit.transform.IsChildOf(enterTrigger.transform))
            {
                return true;
            }
        }
        return false;
    }

    void EnterVehicle()
    {
        isInVehicle = true;

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);

        if (playerCharacter != null)
            playerCharacter.SetActive(false);

        if (vehicleCamera != null)
        {
            vehicleCamera.gameObject.SetActive(true);

            if (vehicleCameraPosition != null)
            {
                vehicleCamera.transform.position = vehicleCameraPosition.position;
                vehicleCamera.transform.rotation = vehicleCameraPosition.rotation;
            }
        }

        if (vehicleCameraScript != null)
        {
            vehicleCameraScript.SetVehicleMode(true);
            vehicleCameraScript.SetUIMode(false);
            vehicleCameraScript.SetFollowTarget(vehicleCameraTarget);
        }

        if (vehicleController != null)
        {
            vehicleController.SetDrivingState(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void ExitVehicle()
    {
        isInVehicle = false;

        if (vehicleController != null)
        {
            vehicleController.SetDrivingState(false);
        }

        if (vehicleCamera != null)
            vehicleCamera.gameObject.SetActive(false);

        if (playerCharacter != null)
        {
            if (exitFromVehiclePosition != null)
            {
                playerCharacter.transform.position = exitFromVehiclePosition.position;
                playerCharacter.transform.rotation = exitFromVehiclePosition.rotation;
            }
            playerCharacter.SetActive(true);
        }

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
            playerCamera.transform.SetParent(playerCharacter.transform);
            playerCamera.transform.localPosition = originalPlayerCameraPosition;
            playerCamera.transform.localRotation = originalPlayerCameraRotation;
        }

        if (vehicleCameraScript != null)
        {
            vehicleCameraScript.SetVehicleMode(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void StartPractice()
    {
        isPracticeMode = true;

        if (practiceButton != null)
            practiceButton.SetActive(false);

        if (vehicleCamera != null)
            vehicleCamera.gameObject.SetActive(false);

        if (playerCharacter != null)
            playerCharacter.SetActive(true);

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
            playerCamera.transform.SetParent(playerCharacter.transform);
            playerCamera.transform.localPosition = originalPlayerCameraPosition;
            playerCamera.transform.localRotation = originalPlayerCameraRotation;
        }

        if (vehicleCameraScript != null)
        {
            vehicleCameraScript.SetUIMode(false);
            vehicleCameraScript.SetVehicleMode(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}