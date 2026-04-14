using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speedX = 5f;
    public float speedY = 0.5f;
    public float speedZ = -1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(
            speedX * Time.deltaTime,
            speedY * Time.deltaTime,
            speedZ * Time.deltaTime
            );
    }
}
