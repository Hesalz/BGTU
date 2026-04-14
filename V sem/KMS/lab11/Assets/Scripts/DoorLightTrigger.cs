using UnityEngine;

public class DoorLightTrigger : MonoBehaviour
{
    public Light Point;

    private void OnTriggerEnter(Collider col)
    {
        if (col.name == "player" && Point != null)
        {
            Point.enabled = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.name == "player" && Point != null)
        {
            Point.enabled = false;
        }
    }
}