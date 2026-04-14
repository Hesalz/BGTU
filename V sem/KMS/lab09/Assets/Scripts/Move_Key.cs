using UnityEngine;

public class Move_Key : MonoBehaviour
{
    public float moveSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
    }
}
