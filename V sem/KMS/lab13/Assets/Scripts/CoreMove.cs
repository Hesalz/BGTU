using UnityEngine;

public class CoreMove : MonoBehaviour
{
    public float speed = 500f;
    public float lifeTime = 5f;
    public GameObject explosionPrefab;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        rb.linearVelocity = transform.up * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goal"))
        {
            Instantiate(
                explosionPrefab,
                collision.contacts[0].point,
                Quaternion.identity
            );

            Destroy(gameObject);
        }
    }
}
