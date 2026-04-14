using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    public GameObject wall;
    public float rotationSpeed = 50f;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wall.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}