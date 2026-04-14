using UnityEngine;

public class Rotation_Rotate : MonoBehaviour
{
    public float angularSpeedY = 90f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, angularSpeedY * Time.deltaTime, 0f, Space.Self);
    }
}
