using UnityEngine;

public class SpotLight : MonoBehaviour
{
    public Light Spot;
    public float rotationSpeed = 30f;

    private void OnTriggerStay(Collider col)
    {
        if (col.name == "player" && Spot != null)
        {
            Spot.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }
    }
}