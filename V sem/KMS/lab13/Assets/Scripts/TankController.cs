using UnityEngine;

public class TankControl : MonoBehaviour
{
    public Transform turret;
    public Transform barrel;

    public float moveSpeed = 40f;
    public float turnSpeed = 90f;

    public float turretSpeed = 120f;

    public float barrelSpeed = 60f;
    public float minBarrelAngle = -25f;
    public float maxBarrelAngle = 10f;

    private float barrelAngle;

    void Update()
    {
        MoveHull();
        RotateTurret();
        RotateBarrel();
    }

    void MoveHull()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        transform.Translate(Vector3.up * v * moveSpeed * Time.deltaTime, Space.Self);
        transform.Rotate(Vector3.forward * h * turnSpeed * Time.deltaTime, Space.Self);
    }

    void RotateTurret()
    {
        if (!turret) return;

        float mouseX = Input.GetAxis("Mouse X");
        turret.Rotate(Vector3.forward * mouseX * turretSpeed * Time.deltaTime, Space.Self);
    }

    void RotateBarrel()
    {
        if (!barrel) return;

        float mouseY = Input.GetAxis("Mouse Y");

        barrelAngle -= mouseY * barrelSpeed * Time.deltaTime;
        barrelAngle = Mathf.Clamp(barrelAngle, minBarrelAngle, maxBarrelAngle);

        barrel.localRotation = Quaternion.Euler(0f, barrelAngle, 0f);
    }
}
