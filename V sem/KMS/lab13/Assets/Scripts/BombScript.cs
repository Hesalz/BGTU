using UnityEngine;

public class BombScript : MonoBehaviour
{
    public GameObject explosionPrefab;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            AudioScript audioManager = FindObjectOfType<AudioScript>();
            if (audioManager != null)
            {
                audioManager.PlayExplosionSound(transform.position);
            }

            Instantiate(
                explosionPrefab,
                collision.contacts[0].point,
                Quaternion.identity
            );

            Destroy(gameObject, 0.1f);
        }
    }
}
