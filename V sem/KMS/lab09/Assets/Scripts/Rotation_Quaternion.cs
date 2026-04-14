using UnityEngine;

public class Rotation_Quaternion : MonoBehaviour
{
    public float speedX = 90f;
    public float speedZ = 60f;

    private Quaternion startPosition;
    private float angleX;
    private float angleZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        angleX += speedX * Time.deltaTime;
        angleZ += speedZ * Time.deltaTime;

        Quaternion rotX = Quaternion.AngleAxis(angleX, Vector3.right);
        Quaternion rotZ = Quaternion.AngleAxis(angleZ, Vector3.forward);

        transform.rotation = startPosition * rotX * rotZ;
    }
}
