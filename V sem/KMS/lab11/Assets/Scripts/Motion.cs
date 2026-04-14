using UnityEngine;

public class Motion : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        movement = transform.TransformDirection(movement);
        movement *= moveSpeed * Time.deltaTime;

        transform.position += movement;

        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(0f, mouseX * rotationSpeed, 0f);
    }
}