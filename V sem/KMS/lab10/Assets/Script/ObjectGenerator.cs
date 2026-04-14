using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    private float tiltAngle = 0f;
    private float tiltSpeed = 50f;
    public GameObject prefub;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GenerateCube();
        }

        if (Input.GetKey(KeyCode.W))
        {
            TiltPlanee();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateSphere();
        }
    }

    void GenerateCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        Renderer planeRenderer = GetComponent<Renderer>();
        Vector3 planeSize = planeRenderer.bounds.size;
        Vector3 planePosition = transform.position;

        float randomX = Random.Range(-planeSize.x / 2, planeSize.x / 2);
        float randomZ = Random.Range(-planeSize.z / 2, planeSize.z / 2);

        Vector3 spawnPosition = new Vector3(
            planePosition.x + randomX,
            planePosition.y + 3f,
            planePosition.z + randomZ
        );

        cube.transform.position = spawnPosition;
        cube.AddComponent<Rigidbody>();
    }

    void GenerateSphere()
    {
        Renderer planeRenderer = GetComponent<Renderer>();
        Vector3 planeSize = planeRenderer.bounds.size;
        Vector3 planePosition = transform.position;

        float randomX = Random.Range(-planeSize.x / 2, planeSize.x / 2);
        float randomZ = Random.Range(-planeSize.z / 2, planeSize.z / 2);

        Vector3 spawnPosition = new Vector3(
            planePosition.x + randomX,
            planePosition.y + 3f,
            planePosition.z + randomZ
        );

        Instantiate(prefub, spawnPosition, Quaternion.identity);
    }

    void TiltPlanee()
    {
        tiltAngle += tiltSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, tiltAngle);
    }
}