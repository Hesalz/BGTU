using UnityEngine;

public class WinchSystem : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform winchPoint;
    public Rigidbody hookRb;
    public LineRenderer lineRenderer;

    [Header("Platform Lock")]
    public Transform loadPoint;
    public float lockDistance = 8f;

    [Header("Cable")]
    public float cableLength = 10f;
    public float minCableLength = 2f;
    public float maxCableLength = 30f;
    public float reelSpeed = 5f;

    [Header("Tow Force")]
    public float towForce = 30000f;
    public float upwardForce = 60000f;

    private bool isDraggingHook;
    private bool hookAttached;
    private bool vehicleLocked;

    private Rigidbody attachedVehicleRb;
    private Transform attachedTowPoint;

    private Collider hookCollider;

    void Start()
    {
        hookCollider = hookRb.GetComponent<Collider>();

        ResetHookToWinch();
    }

    void Update()
    {
        HandleHookDrag();
        HandleCableInput();
        UpdateCableVisual();
        LimitCableLength();

        if (!isDraggingHook && !hookAttached)
            ResetHookToWinch();
    }

    void FixedUpdate()
    {
        PullVehicle();
        CheckVehicleLock();

        if (vehicleLocked && attachedVehicleRb != null)
        {
            Transform vehicleRoot = attachedVehicleRb.transform.root;
            vehicleRoot.position = loadPoint.position;
            vehicleRoot.rotation = loadPoint.rotation;
        }
    }


    void ResetHookToWinch()
    {
        hookRb.isKinematic = true;
        hookRb.useGravity = false;

        hookRb.position = winchPoint.position;
        hookRb.rotation = winchPoint.rotation;

        hookRb.Sleep();
    }


    void HandleHookDrag()
    {
        if (hookAttached) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0)
            );

            if (Physics.Raycast(ray, out RaycastHit hit, 4f))
            {
                if (hit.collider.CompareTag("Hook"))
                {
                    isDraggingHook = true;

                    hookRb.isKinematic = false;
                    hookRb.useGravity = false;
                }
            }
        }

        if (isDraggingHook)
        {
            Vector3 target =
                playerCamera.transform.position +
                playerCamera.transform.forward * 4f;

            hookRb.MovePosition(
                Vector3.Lerp(
                    hookRb.position,
                    target,
                    Time.deltaTime * 15f
                )
            );

            hookRb.linearVelocity = Vector3.zero;
            hookRb.angularVelocity = Vector3.zero;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDraggingHook = false;

            hookRb.useGravity = true;

            if (!hookAttached)
                ResetHookToWinch();
        }
    }


    void HandleCableInput()
    {
        if (Input.GetKey(KeyCode.R))
            cableLength -= reelSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.F))
            cableLength += reelSpeed * Time.deltaTime;

        cableLength = Mathf.Clamp(
            cableLength,
            minCableLength,
            maxCableLength
        );
    }

    void UpdateCableVisual()
    {
        lineRenderer.SetPosition(0, winchPoint.position);

        if (hookAttached && attachedTowPoint != null)
            lineRenderer.SetPosition(1, attachedTowPoint.position);
        else
            lineRenderer.SetPosition(1, hookRb.position);
    }

    void LimitCableLength()
    {
        if (hookAttached) return;

        float dist = Vector3.Distance(
            winchPoint.position,
            hookRb.position
        );

        if (dist > cableLength)
            cableLength = dist;
    }


    void PullVehicle()
    {
        if (!hookAttached) return;
        if (attachedVehicleRb == null) return;
        if (vehicleLocked) return;

        hookRb.position = attachedTowPoint.position;
        hookRb.rotation = attachedTowPoint.rotation;

        Vector3 toWinch =
            winchPoint.position - attachedTowPoint.position;

        float dist = toWinch.magnitude;

        if (dist <= cableLength)
            return;

        Vector3 dir = toWinch.normalized;

        Vector3 pull =
            dir * towForce;

        float tension =
            Mathf.Clamp01(dist - cableLength);

        Vector3 lift =
            Vector3.up * upwardForce * tension;

        Vector3 finalForce =
            pull + lift;

        attachedVehicleRb.AddForceAtPosition(
            finalForce,
            attachedTowPoint.position,
            ForceMode.Force
        );

        attachedVehicleRb.linearVelocity =
            Vector3.Lerp(
                attachedVehicleRb.linearVelocity,
                attachedVehicleRb.linearVelocity * 0.98f,
                Time.fixedDeltaTime * 8f
            );
    }


    void CheckVehicleLock()
    {
        if (vehicleLocked) return;
        if (attachedVehicleRb == null) return;
        if (loadPoint == null) return;

        float dist = Vector3.Distance(
            attachedVehicleRb.position,
            loadPoint.position
        );

        if (dist <= lockDistance)
        {
            LockVehicle();
        }
    }

    void LockVehicle()
    {
        vehicleLocked = true;

        attachedVehicleRb.linearVelocity = Vector3.zero;
        attachedVehicleRb.angularVelocity = Vector3.zero;
        attachedVehicleRb.isKinematic = true;

        Transform vehicleRoot = attachedVehicleRb.transform.root;

        vehicleRoot.position = loadPoint.position;
        vehicleRoot.rotation = loadPoint.rotation;

        Debug.Log($"VEHICLE LOCKED");
    }



    public void AttachToVehicle(
        Rigidbody targetRb,
        Transform towPoint
    )
    {
        if (hookAttached) return;

        attachedVehicleRb = targetRb;
        attachedTowPoint = towPoint;

        Collider[] vehicleCols =
            targetRb.GetComponentsInChildren<Collider>();

        foreach (var col in vehicleCols)
        {
            Physics.IgnoreCollision(
                hookCollider,
                col,
                true
            );
        }

        targetRb.linearVelocity = Vector3.zero;
        targetRb.angularVelocity = Vector3.zero;

        hookRb.isKinematic = true;

        hookAttached = true;
    }

    public bool IsHookAttached()
    {
        return hookAttached;
    }

    public bool IsVehicleLocked()
    {
        return vehicleLocked;
    }

    public bool IsVehicleOnPlatform()
    {
        if (!hookAttached || attachedVehicleRb == null) return false;
        float dist = Vector3.Distance(attachedVehicleRb.position, loadPoint.position);
        return dist <= lockDistance;
    }
}