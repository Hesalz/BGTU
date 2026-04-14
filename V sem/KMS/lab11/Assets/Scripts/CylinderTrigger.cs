using UnityEngine;

public class CylinderTrigger : MonoBehaviour
{
    public Light point1;
    public Light point2;
    public Light point3;

    public GameObject cylinder;

    public float maxIntensity = 5f;
    public float intensityChangeSpeed = 1f;
    public float cylinderRotationSpeed = 30f;

    private float[] startIntensities = new float[3];
    private bool isPlayerInTrigger = false;
    private float intensityTimer = 0f;

    void Start()
    {
        if (point1 != null) startIntensities[0] = point1.intensity;
        if (point2 != null) startIntensities[1] = point2.intensity;
        if (point3 != null) startIntensities[2] = point3.intensity;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "player")
        {
            isPlayerInTrigger = true;
            intensityTimer = 0f;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.name == "player")
        {
            isPlayerInTrigger = false;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.name == "player" && isPlayerInTrigger)
        {
            intensityTimer += Time.deltaTime * intensityChangeSpeed;

            float intensityMultiplier = (Mathf.Sin(intensityTimer) + 1f) / 2f;

            if (point1 != null)
                point1.intensity = Mathf.Lerp(startIntensities[0], maxIntensity, intensityMultiplier);

            if (point2 != null)
                point2.intensity = Mathf.Lerp(startIntensities[1], maxIntensity, intensityMultiplier);

            if (point3 != null)
                point3.intensity = Mathf.Lerp(startIntensities[2], maxIntensity, intensityMultiplier);

            if (cylinder != null)
            {
                cylinder.transform.Rotate(0f, cylinderRotationSpeed * Time.deltaTime, 0f);
            }
        }
    }

    void Update()
    {
        if (!isPlayerInTrigger)
        {
            if (point1 != null)
                point1.intensity = startIntensities[0];

            if (point2 != null)
                point1.intensity = startIntensities[1];

            if (point3 != null)
                point1.intensity = startIntensities[2];
        }
    }
}