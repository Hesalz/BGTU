using UnityEngine;

public class BombScript : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            AudioScript audioManager = FindObjectOfType<AudioScript>();
            if (audioManager != null)
            {
                audioManager.PlayExplosionSound(transform.position);
            }
            Destroy(gameObject, 0.1f);
        }
    }
}