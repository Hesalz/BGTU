using UnityEngine;

public class CubeMotion : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Input.GetKey(KeyCode.E))
            moveVertical = 1f;
        if (Input.GetKey(KeyCode.R))
            moveVertical = -1f;
        if (Input.GetKey(KeyCode.T))
            moveHorizontal = 1f;
        if (Input.GetKey(KeyCode.Y))
            moveHorizontal = -1f;

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        movement *= moveSpeed * Time.deltaTime;

        transform.Translate(movement, Space.World);
    }
}