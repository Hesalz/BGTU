using UnityEngine;

public class RopeDrag : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Grab Settings")]
    public float grabDistance = 4f;
    public float holdDistance = 2f;
    public float moveForce = 600f;

    [Header("Last Rope Segment Name")]
    public string ropeEndName = "RopeEnd";

    private Rigidbody grabbedBody;
    private bool isDragging;

    void Update()
    {
        TryGrab();
        DragObject();
        ReleaseObject();
    }

    void TryGrab()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
            {
                if (hit.collider.gameObject.name == ropeEndName)
                {
                    Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

                    if (rb != null)
                    {
                        grabbedBody = rb;
                        isDragging = true;
                    }
                }
            }
        }
    }

    void DragObject()
    {
        if (!isDragging || grabbedBody == null)
            return;

        Vector3 targetPosition =
            playerCamera.transform.position +
            playerCamera.transform.forward * holdDistance;

        Vector3 direction = targetPosition - grabbedBody.position;

        grabbedBody.linearVelocity = Vector3.zero;
        grabbedBody.angularVelocity = Vector3.zero;

        grabbedBody.AddForce(direction * moveForce * Time.deltaTime, ForceMode.VelocityChange);
    }

    void ReleaseObject()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            grabbedBody = null;
        }
    }
}