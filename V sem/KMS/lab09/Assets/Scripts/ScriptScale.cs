using UnityEngine;

public class ScriptScale : MonoBehaviour
{
    public float scaleSpeedX = 0.5f;
    public float scaleSpeedY = 0.5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 s = transform.localScale;

        s.x += scaleSpeedX * Time.deltaTime;
        s.y += scaleSpeedY * Time.deltaTime;

        transform.localScale = s; 
    }
}
