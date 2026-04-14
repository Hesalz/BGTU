using UnityEngine;

public class BombGenerator : MonoBehaviour
{
    public GameObject bombPrefab;
    public Transform tankTransform;
    public KeyCode bombKey = KeyCode.Q;

    void Update()
    {
        if (Input.GetKeyDown(bombKey))
        {
            GenerateBombs();
        }
    }

    void GenerateBombs()
    {
        int bombCount = Random.Range(5, 11);

        for (int i = 0; i < bombCount; i++)
        {
            Vector3 tankPosition = tankTransform.position;
            Vector3 tankForward = tankTransform.forward;

            float areaSize = 10f;

            Vector3 dropPosition = tankPosition +
                                  tankForward * 20f +
                                  new Vector3(
                                      Random.Range(-areaSize / 2, areaSize / 2),
                                      30f,
                                      Random.Range(-areaSize / 2, areaSize / 2)
                                  );

            GameObject bomb = Instantiate(bombPrefab, dropPosition, Quaternion.identity);

            bomb.transform.rotation = Random.rotation;

            Rigidbody rb = bomb.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }
        }
    }
}