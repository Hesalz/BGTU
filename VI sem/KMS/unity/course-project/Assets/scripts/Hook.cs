using UnityEngine;

public class Hook : MonoBehaviour
{
    public WinchSystem winchSystem;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.transform.root.GetComponent<Rigidbody>();

        if (rb == null) return;
        if (!rb.CompareTag("Towable")) return;

        TowPoint point = other.transform.root.GetComponentInChildren<TowPoint>();

        if (point == null)
        {
            return;
        }

        winchSystem.AttachToVehicle(rb, point.transform);
    }
}