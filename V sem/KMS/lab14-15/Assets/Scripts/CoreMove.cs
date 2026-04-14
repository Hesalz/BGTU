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
            if (explosionPrefab)
            {
                GameObject explosion = Instantiate(
                explosionPrefab,
                collision.contacts[0].point,
                Quaternion.identity
            );

                Destroy(explosion, 2f);
            }
            Destroy(gameObject);
        }

        if (!collision.gameObject.CompareTag("core"))
        {
            if (explosionPrefab)
            {
                GameObject explosion = Instantiate(
                explosionPrefab,
                collision.contacts[0].point,
                Quaternion.identity
            );

                Destroy(explosion, 2f);
            }

            Destroy(gameObject, 0.1f);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (explosionPrefab)
            {
                GameObject explosion = Instantiate(
                explosionPrefab,
                collision.contacts[0].point,
                Quaternion.identity
            );

                Destroy(explosion, 2f);
            }
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            if (explosionPrefab)
            {
                GameObject explosion = Instantiate(
                explosionPrefab,
                collision.contacts[0].point,
                Quaternion.identity
            );

                Destroy(explosion, 2f);
            }
            Destroy(gameObject);
        }
    }
}