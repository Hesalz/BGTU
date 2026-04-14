using UnityEngine;

public class Input_GetAxis : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;

    private float verticalAngle = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        transform.Translate(x * moveSpeed * Time.deltaTime, 0, z * moveSpeed * Time.deltaTime);


        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");


        verticalAngle -= mx * rotationSpeed * Time.deltaTime;
        verticalAngle = Mathf.Clamp(verticalAngle, 0f, 90f);

        transform.rotation = Quaternion.Euler(verticalAngle, transform.eulerAngles.y + my * rotationSpeed * Time.deltaTime, 0f);
    }
}
