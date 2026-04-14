using UnityEngine;

public class PlayerTank : MonoBehaviour
{

    void Start()
    {
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("core"))
        {
            Destroy(col.gameObject);
        }
    }
}