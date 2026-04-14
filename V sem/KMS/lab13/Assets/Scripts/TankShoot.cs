using UnityEngine;

public class TankShoot : MonoBehaviour
{
    public GameObject corePrefab;
    public float shootDistance = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 spawnPosition =
                transform.position + transform.up * shootDistance;

            Instantiate(corePrefab, spawnPosition, transform.rotation);
        }
    }
}
