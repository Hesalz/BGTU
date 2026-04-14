using UnityEngine;

public class TankShoot : MonoBehaviour
{
    public GameObject corePrefab;
    public float shootDistance = 15f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 spawnPosition = transform.position + transform.up * shootDistance;
            GameObject core = Instantiate(corePrefab, spawnPosition, transform.rotation);

            core.tag = "core";
        }
    }
}