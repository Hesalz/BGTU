using UnityEngine;

public class PointTrigger : MonoBehaviour
{
    public Light pointLight;
    public float triggerIntensity = 50000f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pointLight.intensity = triggerIntensity;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pointLight.intensity = 0;
        }
    }
}