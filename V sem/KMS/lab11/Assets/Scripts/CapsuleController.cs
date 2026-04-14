using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSpeed = 2f;

    public GameObject targetCube;
    public KeyCode textureKey = KeyCode.T;

    public Texture cube1Texture;
    public Texture cube2Texture;

    private float rotationX = 0f;

    void Update()
    {
        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.Translate(x, 0, z);

        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.eulerAngles = new Vector3(rotationX, transform.eulerAngles.y + mouseX, 0);

        if (Input.GetKeyDown(textureKey) && targetCube != null)
        {
            targetCube.GetComponent<Renderer>().material.mainTexture = cube1Texture;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.CompareTag("Cube1") || obj.CompareTag("Cube2"))
        {
            Renderer rend = obj.GetComponent<Renderer>();
            rend.material.color = Random.ColorHSV();

            if (obj.CompareTag("Cube1") && cube1Texture != null)
            {
                rend.material.mainTexture = cube1Texture;
            }
            else if (obj.CompareTag("Cube2") && cube2Texture != null)
            {
                rend.material.mainTexture = cube2Texture;
            }
        }
    }
}